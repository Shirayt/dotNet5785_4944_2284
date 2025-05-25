namespace BlImplementation;
using BlApi;
using BO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public IEnumerable<int> GetCallQuantitiesByStatus()
    {
        var callStatuses = Helpers.CallManager.GetCallStatuses();

        var callCountsByStatus = callStatuses
            .GroupBy(cs => cs.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        return new[]
        {
        CallStatus.Open,
        CallStatus.InProcessing,
        CallStatus.Closed,
        CallStatus.Expired,
        CallStatus.OpenAtRisk
        }
        .Select(status => callCountsByStatus.TryGetValue(status, out var count) ? count : 0);
    }

    public IEnumerable<BO.CallInList> GetCallsList(BO.CallInListFields? filterField, object? filterValue, BO.CallInListFields? sortField)
    {
        var calls = _dal.Call.ReadAll();

        // Group by CallId and select the last allocation
        var callsList = calls
            .GroupBy(c => c.Id)
            .Select(g => g.OrderByDescending(c => c.OpenTime).First())
            .Select(c => new BO.CallInList
            {
                Id = c.Id,
                CallId = c.Id,
                CallType = (BO.CallType)c.CallType,
                OpenTime = c.OpenTime,
                RestTimeForCall = Helpers.CallManager.CalculateRestTimeForCall(c),
                LastVolunteerName = Helpers.CallManager.GetLastVolunteerName(c),
                RestTimeForTreatment = Helpers.CallManager.CalculateRestTimeForTreatment(c),
                Status = Helpers.CallManager.GetCallStatus(c).Status,
                AllocationsAmount = Helpers.CallManager.GetAllocationsAmount(c.Id)
            });

        // Apply filtering if filterField is provided
        if (filterField.HasValue && filterValue != null)
        {
            var property = typeof(BO.CallInList).GetProperty(filterField.ToString());
            callsList = callsList.Where(c => property.GetValue(c) == filterValue);
        }

        // Apply sorting if sortField is provided
        if (sortField != null)
        {
            var property = typeof(BO.CallInList).GetProperty(sortField.ToString());
            if (property != null)
            {
                callsList = callsList.OrderBy(c => property.GetValue(c));
            }
        }
        else
        {
            callsList = callsList.OrderBy(c => c.CallId); // Default sorting by CallId
        }

        return callsList;
    }
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId);
            var assignments = _dal.Assignment.ReadAll();

            var BOCall = new BO.Call
            {
                Id = call.Id,
                CallType = (BO.CallType)call.CallType,
                Description = call.Description,
                FullAddress = call.FullAddress,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                OpenTime = call.OpenTime,
                MaxEndTime = call.MaxEndTime,
                Status = Helpers.CallManager.GetCallStatus(call).Status,
                CallAssignInList = assignments
                              .Where(assign => assign.CallId == callId)
                              .Select(assign => new BO.CallAssignInList
                              {
                                  VolunteerId = assign.VolunteerId,
                                  FullName = Helpers.CallManager.GetLastVolunteerName(call),
                                  StartTime = assign.StartTime,
                                  EndTime = assign.EndTime,
                                  Status = (BO.AssignmentStatus)assign.Status
                              }).ToList() ?? null
            };

            return BOCall;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during get call details in BL: ", ex);
        }
    }
    public void UpdateCallDetails(BO.Call call)
    {
        try
        {
            Helpers.CallManager.ValidateBOCallData(call);

            var doCall = new DO.Call
            {
                Id = call.Id,
                CallType = (DO.CallType)call.CallType,
                Description = call.Description,
                FullAddress = call.FullAddress,
                Latitude = call.Latitude ?? 0,
                Longitude = call.Longitude ?? 0,
                OpenTime = call.OpenTime,
                MaxEndTime = call.MaxEndTime
            };

            _dal.Call.Update(doCall);
            CallManager.Observers.NotifyItemUpdated(doCall.Id); //stage 5                                                    
            CallManager.Observers.NotifyListUpdated(); //stage 5                                                    
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during update call details in BL: ", ex);
        }
    }
    public void DeleteCall(int callId)
    {
        try
        {
            // Check if the call exists
            var call = _dal.Call.Read(callId);
            var assignments = _dal.Assignment.ReadAll();

            //using LINQ to object
            var assignedCalls = (from assign in assignments
                                 where assign.CallId == callId
                                 select assign).ToList();

            // Check if the call is open and has not been assigned to a volunteer
            if (Helpers.CallManager.GetCallStatus(call).Status != BO.CallStatus.Open || assignedCalls.Any())
            {
                // If the call is not open or has been assigned to a volunteer, it cannot be deleted
                throw new InvalidOperationException("Cannot delete a call that is not 'Open' or has been assigned to a volunteer.");
            }


            // If everything is fine, proceed with deleting the call
            _dal.Call.Delete(callId);
            CallManager.Observers.NotifyListUpdated(); //stage 5                                                    
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during call deletion process in BL: ", ex);
        }
    }
    public void AddCall(BO.Call call)
    {
        try
        {
            // Validate the data
            Helpers.CallManager.ValidateBOCallData(call);

            // Create data object
            var doCall = new DO.Call
            {
                CallType = (DO.CallType)call.CallType,
                Description = call.Description,
                FullAddress = call.FullAddress,
                Latitude = call.Latitude ?? 0,
                Longitude = call.Longitude ?? 0,
                OpenTime = call.OpenTime,
                MaxEndTime = call.MaxEndTime
            };

            _dal.Call.Create(doCall);
            CallManager.Observers.NotifyListUpdated(); //stage 5                                                    
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Somthing went wrong during call addition in BL: ", ex);
        }
    }
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.FilterAndSortByFields? filterType, BO.FilterAndSortByFields? sortField)
    {
        var calls = _dal.Call.ReadAll();
        var assignments = _dal.Assignment.ReadAll();

        // list of assignments that the volunteer was assigned to.
        var volunteerassignments = assignments.Where(a => a.VolunteerId == volunteerId);

        //using LINQ to object
        var closedCalls = from c in calls
                          where volunteerassignments.Select(a => a.CallId).Contains(c.Id) // a volunteer call
                          && (Helpers.CallManager.GetCallStatus(c).Status == BO.CallStatus.Closed
                          || Helpers.CallManager.GetCallStatus(c).Status == BO.CallStatus.Expired) // the call was closed
                          select new BO.ClosedCallInList
                          {
                              Id = c.Id,
                              CallType = (BO.CallType)c.CallType,
                              FullAddress = c.FullAddress,
                              OpenTime = c.OpenTime,
                              EndTime = Helpers.CallManager.GetAssignmentForCall(volunteerassignments, c.Id)?.EndTime ?? AdminManager.Now,
                              TreatmentStartTime = Helpers.CallManager.GetAssignmentForCall(volunteerassignments, c.Id)?.StartTime,
                              Status = (BO.AssignmentStatus)Helpers.CallManager.GetAssignmentForCall(volunteerassignments, c.Id)?.Status
                          };


        // Apply filter and sort using the helper function
        closedCalls = Helpers.CallManager.ApplyFilterAndSort(closedCalls.AsQueryable(), filterType, sortField);

        return closedCalls;
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsByVolunteer(int volunteerId, BO.FilterAndSortByFields? filterType, BO.FilterAndSortByFields? sortField)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
            var openCalls = _dal.Call.ReadAll()
     .Where(c =>
         (CallManager.GetCallStatus(c).Status == BO.CallStatus.Open || CallManager.GetCallStatus(c).Status == BO.CallStatus.OpenAtRisk))
     .Select(c => new BO.OpenCallInList
     {
         Id = c.Id,
         CallType = (BO.CallType)c.CallType,
         Description = c.Description,
         FullAddress = c.FullAddress,
         OpenTime = c.OpenTime,
         MaxEndTime = c.MaxEndTime,
         DistanceFromVolunteer = Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude, c.Latitude, c.Longitude)
     });

            return sortField != null
    ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString()!)?.GetValue(c))
    : openCalls.OrderBy(c => c.Id);

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException(ex.Message, ex);
        }
    }


    public void MarkCallAsCompleted(int volunteerId, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(assignmentId);

            // assignment belongs to volunteer
            if (assignment.VolunteerId != volunteerId)
            {
                throw new UnauthorizedAccessException("Volunteer is not authorized to complete this assignment.");
            }

            var call = _dal.Call.Read(assignment.CallId);
            var callStatus = Helpers.CallManager.GetCallStatus(call).Status;

            // checks if call is open
            if (!(callStatus == BO.CallStatus.Open || callStatus == BO.CallStatus.OpenAtRisk) || assignment.EndTime != null)
            {
                throw new InvalidOperationException("Assignment is not open for completion.");
            }

            //create an object with updated status,End time fields
            var updatedAssignment = assignment with
            {
                Status = DO.AssignmentStatus.Completed,
                EndTime = AdminManager.Now
            };

            _dal.Assignment.Update(updatedAssignment);
            AssignmentManager.Observers.NotifyItemUpdated(assignmentId); //stage 5
            AssignmentManager.Observers.NotifyListUpdated(); //stage 5                                                    

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during Mark Call As Completed in BL: ", ex);
        }
    }
    public void CancelCallAssignment(int requesterId, int callId)
    {
        try
        {
            var assignment = _dal.Assignment.ReadAll().FirstOrDefault(a => a.CallId == callId);
            var requesterVolunteer = _dal.Volunteer.Read(requesterId);

            //only Manager or the assignment volunteer authorized to cancel an assignment
            if (requesterVolunteer.Role != DO.Role.Manager && assignment.VolunteerId != requesterId)
            {
                throw new UnauthorizedAccessException("Requester is not authorized to cancel this assignment.");
            }

            var call = _dal.Call.Read(callId);
            var callStatus = Helpers.CallManager.GetCallStatus(call).Status;

            //the assignment must be open for cancellation
            if (!(callStatus == BO.CallStatus.Open || callStatus == BO.CallStatus.OpenAtRisk) || assignment.EndTime != null)
            {
                throw new InvalidOperationException("Call is not open for cancellation.");
            }

            //create an object with updated status,End time fields
            var updatedAssignment = assignment with
            {
                Status = assignment.VolunteerId == requesterId ? DO.AssignmentStatus.SelfCancelled : DO.AssignmentStatus.ManagerCancelled,
                EndTime = AdminManager.Now
            };

            _dal.Assignment.Update(updatedAssignment);
            AssignmentManager.Observers.NotifyItemUpdated(updatedAssignment.Id); //stage 5                                                    
            AssignmentManager.Observers.NotifyListUpdated(); //stage 5                                                    

        }

        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during Cancel Call Assignment in BL: ", ex);
        }
    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId);
            var volunteer = _dal.Volunteer.Read(volunteerId);
            var assignments = _dal.Assignment.ReadAll();
            var callStatus = Helpers.CallManager.GetCallStatus(call).Status;

            //the call is not open for treatment 
            if (!(callStatus == BO.CallStatus.Open || callStatus == BO.CallStatus.OpenAtRisk))
            {
                throw new InvalidOperationException("Call is already in progress or closed.");
            }

            var existingAssignments = assignments.Where(a => a.CallId == callId
                                                            && !(Helpers.CallManager.GetCallStatus(_dal.Call.Read(callId)).Status == BO.CallStatus.Open
                                                            || Helpers.CallManager.GetCallStatus(_dal.Call.Read(callId)).Status == BO.CallStatus.OpenAtRisk));

            //Call is already assigned
            if (existingAssignments.Any())
            {
                throw new InvalidOperationException("Call is already assigned.");
            }

            //"Call has expired.
            if (call.MaxEndTime < AdminManager.Now)
            {
                throw new InvalidOperationException("Call has expired.");
            }

            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartTime = AdminManager.Now,
                EndTime = null,
                Status = null

            };

            _dal.Assignment.Create(newAssignment);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Somthing went wrong during Select Call For Treatment in BL: ", ex);
        }
    }

    public static (double, double) GetCoordinatesFromAddress(string address)
    {
        string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";
        using var client = new HttpClient();
        string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        var response = client.GetAsync(url).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid address or API error.");

        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            throw new Exception("Address not found.");

        var root = doc.RootElement[0];

        // convert values to double
        double latitude = double.Parse(root.GetProperty("lat").GetString());
        double longitude = double.Parse(root.GetProperty("lon").GetString());

        return (latitude, longitude);
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
   CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
   CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
   CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
   CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}
