namespace BlImplementation;
using BlApi;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    //private IEnumerable<DO.Call> calls;
    //private IEnumerable<DO.Assignment> assignments;
    public IEnumerable<int> GetCallQuantitiesByStatus()
    {
        // Get call statuses using a helper function
        var callStatuses = Helpers.CallManager.GetCallStatuses();

        // Group calls by status and count the number of occurrences for each status
        var callCountsByStatus = callStatuses
            .GroupBy(cs => cs.Status)       // Group calls by status
            .OrderBy(group => group.Key)    // Sort by status order (Open, OpenAtRisk, InProcessing, Closed, Expired)
            .Select(group => group.Count()) // Count calls in each status
            .ToArray();                     // Convert result to an array

        return callCountsByStatus;
    }
    public IEnumerable<BO.CallInList> GetCallsList(BO.CallType? filterField, object? filterValue, string? sortField)
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
            callsList = callsList.Where(c => c.CallType == filterField);
        }

        // Apply sorting if sortField is provided
        if (!string.IsNullOrEmpty(sortField))
        {
            var property = typeof(BO.CallInList).GetProperty(sortField);
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

            ///לעדכן את הקאורדינטות של וולנטיר שקשור לקריאה הזו 
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
            if ((Helpers.CallManager.GetCallStatus(call).Status) != BO.CallStatus.Open || !assignedCalls.Any())
            {
                // If the call is not open or has been assigned to a volunteer, it cannot be deleted
                throw new InvalidOperationException("Cannot delete a call that is not 'Open' or has been assigned to a volunteer.");
            }

            // If everything is fine, proceed with deleting the call
            _dal.Call.Delete(callId);
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

            if (_dal.Call.Read(call.Id) != null)
            {
                throw new InvalidOperationException($"A call with ID {call.Id} already exists.");
            }

            // Create data object
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

            _dal.Call.Create(doCall);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Somthing went wrong during call addition in BL: ", ex);
        }
    }
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? filterType, BO.AssignmentStatus? sortField)
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
                              EndTime = Helpers.CallManager.GetAssignmentForCall(volunteerassignments, c.Id)?.EndTime ?? ClockManager.Now,
                              TreatmentStartTime = Helpers.CallManager.GetAssignmentForCall(volunteerassignments, c.Id)?.StartTime,
                              Status = (BO.AssignmentStatus)Helpers.CallManager.GetAssignmentForCall(volunteerassignments, c.Id)?.Status
                          };


        // Apply filter and sort using the helper function
        closedCalls = Helpers.CallManager.ApplyFilterAndSort(closedCalls.AsQueryable(), filterType, sortField);

        return closedCalls;
    }
    public IEnumerable<BO.OpenCallInList> GetOpenCallsByVolunteer(int volunteerId, BO.CallType? filterType, BO.CallType? sortField)
    {

        var calls = _dal.Call.ReadAll();
        var assignments = _dal.Assignment.ReadAll();

        // list of assignments that the volunteer was assigned to.
        var volunteerassignments = assignments.Where(a => a.VolunteerId == volunteerId);

        var openCalls = calls
            .Where(c => volunteerassignments.Select(a => a.CallId).Contains(c.Id) // a volunteer call
                   && (Helpers.CallManager.GetCallStatus(c).Status == BO.CallStatus.Open
                   || Helpers.CallManager.GetCallStatus(c).Status == BO.CallStatus.OpenAtRisk
                   || Helpers.CallManager.GetCallStatus(c).Status == BO.CallStatus.InProcessing)) //the call is open 
            .Select(c => new BO.OpenCallInList
            {
                Id = c.Id,
                CallType = (BO.CallType)c.CallType,
                Description = c.Description,
                FullAddress = c.FullAddress,
                OpenTime = c.OpenTime,
                MaxEndTime = c.MaxEndTime,
                DistanceFromVolunteer = Helpers.CallManager.GetDistanceFromVolunteer(volunteerId, c)
            });

        // Apply filter and sort using the helper function
        openCalls = Helpers.CallManager.ApplyFilterAndSort(openCalls.AsQueryable(), filterType, sortField);

        return openCalls;
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
                EndTime = ClockManager.Now
            };

            _dal.Assignment.Update(updatedAssignment);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Somthing went wrong during Mark Call As Completed in BL: ", ex);
        }
    }
    public void CancelCallAssignment(int requesterId, int assignmentId)
    {
        try
        {
            var assignment = _dal.Assignment.Read(assignmentId);
            var requesterVolunteer = _dal.Volunteer.Read(requesterId);

            //only Manager or the assignment volunteer authorized to cancel an assignment
            if (requesterVolunteer.Role != DO.Role.Manager && assignment.VolunteerId != requesterId)
            {
                throw new UnauthorizedAccessException("Requester is not authorized to cancel this assignment.");
            }

            var call = _dal.Call.Read(assignment.CallId);
            var callStatus = Helpers.CallManager.GetCallStatus(call).Status;

            //the assignment must be open for cancellation
            if (!(callStatus == BO.CallStatus.Open || callStatus == BO.CallStatus.OpenAtRisk) || assignment.EndTime != null)
            {
                throw new InvalidOperationException("Call is not open for cancellation.");
            }

            //create an object with updated status,End time fields
            var updatedAssignment = assignment with
            {
                Status = assignment.VolunteerId == requesterId ? DO.AssignmentStatus.ManagerCancelled : DO.AssignmentStatus.SelfCancelled,
                EndTime = ClockManager.Now
            };

            _dal.Assignment.Update(updatedAssignment);
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
            if (call.MaxEndTime < ClockManager.Now)
            {
                throw new InvalidOperationException("Call has expired.");
            }

            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartTime = ClockManager.Now,
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
}
