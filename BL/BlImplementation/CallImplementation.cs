namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    private IEnumerable<DO.Call> calls;
    private IEnumerable<DO.Assignment> assignments;
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

            if (call != null)
            {
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
            else
            {
                // If the call is not found
                throw new Exception("Call not found.");
            }
        }
        catch (Exception ex)
        {
            throw new Exception("BO.Error while fetching call details.", ex);
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
        catch (Exception ex)
        {
            throw new Exception("Error during call deletion process: " + ex.Message);
        }
    }
    public void DeleteCall(int callId)
    {
        try
        {
            // Check if the call exists
            var call = _dal.Call.Read(callId);
            var assignments = _dal.Assignment.ReadAll();

            var assignedCalls = assignments.Where(assign => assign.CallId == callId).ToList();

            // Check if the call is open and has not been assigned to a volunteer
            if ((Helpers.CallManager.GetCallStatus(call).Status) != BO.CallStatus.Open || !assignedCalls.Any())
            {
                // If the call is not open or has been assigned to a volunteer, it cannot be deleted
                throw new InvalidOperationException("Cannot delete a call that is not 'Open' or has been assigned to a volunteer.");
            }

            // If everything is fine, proceed with deleting the call
            _dal.Call.Delete(callId);
        }
        catch (Exception ex)
        {
            throw new Exception("Error during call deletion process: " + ex.Message);
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
        catch (Exception ex) // Exception for existing ID
        {
            throw new Exception($"A call with ID {call.Id} already exists.", ex);
        }

    }
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? filterType, BO.AssignmentStatus? sortField)
    {
        var calls = _dal.Call.ReadAll();

        var closedCalls = calls
            .Select(c => new { Call = c, StatusInfo = Helpers.CallManager.GetCallStatus(c) })
            .Where(c => c.StatusInfo.Status == BO.CallStatus.Closed|| c.StatusInfo.Status == BO.CallStatus.Expired)
            .Select(c => new BO.ClosedCallInList
            {
                 Id = c.Call.Id,
                 CallType = (BO.CallType)c.Call.CallType, 
                 FullAddress = c.Call.FullAddress,
                 OpenTime = c.Call.OpenTime,
                 EndTime = c.Call.MaxEndTime??DateTime.Now,
                 TreatmentStartTime = ,
                 Status = c.StatusInfo.Status
            });

        // filter by call type 
        if (filterType != null)
        {
            closedCalls = closedCalls.Where(c => c.CallType == filterType);
        }

      
        if (sortField != null)
        {
            closedCalls = closedCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString()).GetValue(c));
        }
        else
        {
            closedCalls = closedCalls.OrderBy(c => c.Id); //sort by call Id 
        }

        return closedCalls;
    }
    ////////////////////////////////////////////////
    public void CancelCallAssignment(int volunteerId, int assignmentId)
    {
        var assignment = _assignments.FirstOrDefault(a => a.Id == assignmentId && a.VolunteerId == volunteerId);
        if (assignment != null)
        {
            _assignments.Remove(assignment); // הסרת ההקצאה מהרשימה
        }
        else
        {
            throw new Exception("Assignment not found.");
        }
    }
    public IEnumerable<BO.OpenCallInList> GetOpenCallsByVolunteer(int volunteerId, Enum? filterType, Enum? sortField)
    {
        var openCalls = _calls.Where(c => c.Status == BO.CallStatus.Open && c.VolunteerId == volunteerId)
                              .Select(c => new BO.OpenCallInList
                              {
                                  Id = c.Id,
                                  VolunteerId = c.VolunteerId,
                                  Status = c.Status
                              });

        // סינון ומיון אם יש צורך
        if (filterType != null)
        {
            openCalls = openCalls.Where(c => c.GetType().GetProperty(filterType.ToString()).GetValue(c) != null);
        }

        if (sortField != null)
        {
            openCalls = openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString()).GetValue(c));
        }

        return openCalls;
    }

    public void MarkCallAsCompleted(int volunteerId, int assignmentId)
    {
        var assignment = _assignments.FirstOrDefault(a => a.Id == assignmentId && a.VolunteerId == volunteerId);
        if (assignment != null)
        {
            assignment.Status = BO.AssignmentStatus.Completed; // עדכון סטטוס ההקצאה
            var call = _calls.FirstOrDefault(c => c.Id == assignment.CallId);
            if (call != null)
            {
                call.Status = BO.CallStatus.Closed; // סגירת הקריאה
            }
        }
        else
        {
            throw new Exception("Assignment not found.");
        }
    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        var call = _calls.FirstOrDefault(c => c.Id == callId);
        if (call != null && call.Status == BO.CallStatus.Open)
        {
            call.Status = BO.CallStatus.InProgress; // שינוי סטטוס הקריאה
            _assignments.Add(new BO.Assignment
            {
                VolunteerId = volunteerId,
                CallId = callId,
                Status = BO.AssignmentStatus.InProgress
            });
        }
        else
        {
            throw new Exception("Call is already in progress or closed.");
        }
    }
}
