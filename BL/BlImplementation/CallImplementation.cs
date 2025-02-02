namespace BlImplementation;
using BlApi;
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


    public void AddCall(BO.Call call)
    {
        _dal.Call.Create((DO.Call)call); // הוספת קריאה לרשימה
    }

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

    public void DeleteCall(int callId)
    {
        var call = _calls.FirstOrDefault(c => c.Id == callId);
        if (call != null)
        {
            _calls.Remove(call); // הסרת קריאה מהרשימה
        }
        else
        {
            throw new Exception("Call not found.");
        }
    }

    public BO.Call GetCallDetails(int callId)
    {
        var call = _calls.FirstOrDefault(c => c.Id == callId);
        if (call != null)
        {
            return call; // מחזיר את פרטי הקריאה
        }
        else
        {
            throw new Exception("Call not found.");
        }
    }



    public IEnumerable<BO.CallInList> GetCallsList(Enum? filterField, object? filterValue, Enum? sortField)
    {
        IEnumerable<BO.CallInList> callsList = _calls.Select(c => new BO.CallInList
        {
            Id = c.Id,
            Status = c.Status,
            VolunteerId = c.VolunteerId
        });

        // סינון אם יש צורך
        if (filterField != null && filterValue != null)
        {
            callsList = callsList.Where(c => c.GetType().GetProperty(filterField.ToString()).GetValue(c).Equals(filterValue));
        }

        // מיון אם יש צורך
        if (sortField != null)
        {
            callsList = callsList.OrderBy(c => c.GetType().GetProperty(sortField.ToString()).GetValue(c));
        }

        return callsList;
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, Enum? filterType, Enum? sortField)
    {
        var closedCalls = _calls.Where(c => c.Status == BO.CallStatus.Closed && c.VolunteerId == volunteerId)
                                .Select(c => new BO.ClosedCallInList
                                {
                                    Id = c.Id,
                                    VolunteerId = c.VolunteerId,
                                    Status = c.Status
                                });

        // סינון ומיון אם יש צורך
        if (filterType != null)
        {
            closedCalls = closedCalls.Where(c => c.GetType().GetProperty(filterType.ToString()).GetValue(c) != null);
        }

        if (sortField != null)
        {
            closedCalls = closedCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString()).GetValue(c));
        }

        return closedCalls;
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

    public void UpdateCallDetails(BO.Call call)
    {
        var existingCall = _calls.FirstOrDefault(c => c.Id == call.Id);
        if (existingCall != null)
        {
            existingCall.Description = call.Description;
            existingCall.Status = call.Status;
            existingCall.VolunteerId = call.VolunteerId;
            // עדכון פרטי הקריאה
        }
        else
        {
            throw new Exception("Call not found.");
        }
    }
}
