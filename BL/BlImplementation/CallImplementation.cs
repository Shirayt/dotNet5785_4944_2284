namespace BlImplementation;
using BlApi;
using BO;
using DO;
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
        IEnumerable<DO.Call> calls;
        lock (AdminManager.blMutex)
            calls = _dal.Call.ReadAll();

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

        if (filterField.HasValue && filterValue != null)
        {
            var property = typeof(BO.CallInList).GetProperty(filterField.ToString());
            callsList = callsList.Where(c => property.GetValue(c) == filterValue);
        }

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
            callsList = callsList.OrderBy(c => c.CallId);
        }

        return callsList;
    }

    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            DO.Call call;
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.blMutex)
            {
                call = _dal.Call.Read(callId);
                assignments = _dal.Assignment.ReadAll();
            }

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
                                  FullName = Helpers.VolunteerManager.GetVolunteerFullName(assign.VolunteerId),
                                  StartTime = assign.StartTime,
                                  EndTime = assign.EndTime,
                                  Status = assign.Status.HasValue ? (BO.AssignmentStatus?)assign.Status.Value : null
                              }).ToList() ?? null
            };

            return BOCall;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Somthing went wrong during get call details in BL: ", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException(ex.Message, ex);
        }
    }

    public async Task UpdateCallDetails(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

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

            lock (AdminManager.blMutex)
                _dal.Call.Update(doCall);

            CallManager.Observers.NotifyItemUpdated(doCall.Id);
            CallManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Somthing went wrong during update call details in BL: ", ex);
        }
    }

    public void DeleteCall(int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        try
        {
            DO.Call call;
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.blMutex)
            {
                call = _dal.Call.Read(callId);
                assignments = _dal.Assignment.ReadAll();
            }

            var assignedCalls = (from assign in assignments
                                 where assign.CallId == callId
                                 select assign).ToList();

            if (Helpers.CallManager.GetCallStatus(call).Status != BO.CallStatus.Open || assignedCalls.Any())
            {
                throw new InvalidOperationException("Cannot delete a call that is not 'Open' or has been assigned to a volunteer.");
            }

            lock (AdminManager.blMutex)
                _dal.Call.Delete(callId);

            CallManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Somthing went wrong during call deletion process in BL: ", ex);
        }
    }

    public void AddCall(BO.Call call)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        try
        {
            Helpers.CallManager.ValidateBOCallData(call);

            var doCall = new DO.Call
            {
                CallType = (DO.CallType)call.CallType,
                Description = call.Description,
                FullAddress = call.FullAddress,
                Latitude = call.Latitude ?? 0,
                Longitude = call.Longitude ?? 0,
                OpenTime = AdminManager.Now,
                MaxEndTime = call.MaxEndTime
            };

            lock (AdminManager.blMutex)
                _dal.Call.Create(doCall);

            CallManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException("Somthing went wrong during call addition in BL: ", ex);
        }
    }
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.FilterAndSortByFields? filterType, BO.FilterAndSortByFields? sortField)
    {
        IEnumerable<DO.Call> calls;
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.blMutex)
        {
            calls = _dal.Call.ReadAll();
            assignments = _dal.Assignment.ReadAll();
        }

        var volunteerassignments = assignments.Where(a => a.VolunteerId == volunteerId);

        var closedCalls = from c in calls
                          where volunteerassignments.Select(a => a.CallId).Contains(c.Id)
                          && (Helpers.CallManager.GetCallStatus(c).Status == BO.CallStatus.Closed
                          || Helpers.CallManager.GetCallStatus(c).Status == BO.CallStatus.Expired)
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

        closedCalls = Helpers.CallManager.ApplyFilterAndSort(closedCalls.AsQueryable(), filterType, sortField);

        return closedCalls;
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsByVolunteer(int volunteerId, BO.FilterAndSortByFields? filterType, BO.FilterAndSortByFields? sortField)
    {
        try
        {
            DO.Volunteer volunteer;
            IEnumerable<DO.Call> allCalls;
            lock (AdminManager.blMutex)
            {
                volunteer = _dal.Volunteer.Read(volunteerId);
                allCalls = _dal.Call.ReadAll();
            }

            var openCalls = allCalls
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
        AdminManager.ThrowOnSimulatorIsRunning();

        try
        {
            DO.Assignment assignment;
            DO.Call call;
            lock (AdminManager.blMutex)
            {
                assignment = _dal.Assignment.Read(assignmentId);
                call = _dal.Call.Read(assignment.CallId);
            }

            if (assignment.VolunteerId != volunteerId)
                throw new UnauthorizedAccessException("Volunteer is not authorized to complete this assignment.");

            var callStatus = Helpers.CallManager.GetCallStatus(call).Status;

            if (!(callStatus == BO.CallStatus.InProcessing) || assignment.EndTime != null)
                throw new InvalidOperationException("Assignment is not open for completion.");

            var updatedAssignment = assignment with
            {
                Status = DO.AssignmentStatus.Completed,
                EndTime = AdminManager.Now
            };

            lock (AdminManager.blMutex)
                _dal.Assignment.Update(updatedAssignment);

            CallManager.Observers.NotifyItemUpdated(call.Id);
            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Somthing went wrong during Mark Call As Completed in BL: ", ex);
        }
    }

    public void CancelCallAssignment(int requesterId, int callId)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        try
        {
            DO.Assignment assignment;
            DO.Volunteer requesterVolunteer;
            DO.Call call;
            lock (AdminManager.blMutex)
            {
                assignment = _dal.Assignment.ReadAll().OrderByDescending(a => a.Id).FirstOrDefault(a => a.CallId == callId);
                requesterVolunteer = _dal.Volunteer.Read(requesterId);
                call = _dal.Call.Read(callId);
            }

            if (requesterVolunteer.Role != DO.Role.Manager && assignment.VolunteerId != requesterId)
                throw new UnauthorizedAccessException("Requester is not authorized to cancel this assignment.");

            var callStatus = Helpers.CallManager.GetCallStatus(call).Status;

            if (!(callStatus == BO.CallStatus.InProcessing) || assignment.EndTime != null)
                throw new InvalidOperationException("Call is not open for cancellation.");

            var updatedAssignment = assignment with
            {
                Status = assignment.VolunteerId == requesterId ? DO.AssignmentStatus.SelfCancelled : DO.AssignmentStatus.ManagerCancelled,
                EndTime = AdminManager.Now
            };

            lock (AdminManager.blMutex)
                _dal.Assignment.Update(updatedAssignment);

            CallManager.Observers.NotifyItemUpdated(call.Id);
            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(requesterId);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Somthing went wrong during Cancel Call Assignment in BL: ", ex);
        }
    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            DO.Call call;
            DO.Volunteer volunteer;
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.blMutex)
            {
                call = _dal.Call.Read(callId);
                volunteer = _dal.Volunteer.Read(volunteerId);
                assignments = _dal.Assignment.ReadAll();
            }

            var callStatus = Helpers.CallManager.GetCallStatus(call).Status;

            if (!(callStatus == BO.CallStatus.Open || callStatus == BO.CallStatus.OpenAtRisk))
                throw new InvalidOperationException("Call is already in progress or closed.");

            var existingAssignments = assignments.Where(a => a.CallId == callId
                                                            && !(Helpers.CallManager.GetCallStatus(call).Status == BO.CallStatus.Open
                                                            || Helpers.CallManager.GetCallStatus(call).Status == BO.CallStatus.OpenAtRisk));

            if (existingAssignments.Any())
                throw new InvalidOperationException("Call is already assigned.");

            if (call.MaxEndTime < AdminManager.Now)
                throw new InvalidOperationException("Call has expired.");

            var newAssignment = new DO.Assignment
            {
                CallId = callId,
                VolunteerId = volunteerId,
                StartTime = AdminManager.Now,
                EndTime = null,
                Status = null
            };

            lock (AdminManager.blMutex)
                _dal.Assignment.Create(newAssignment);

            CallManager.Observers.NotifyItemUpdated(callId);
            CallManager.Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException("Somthing went wrong during Select Call For Treatment in BL: ", ex);
        }
    }

    public IEnumerable<BO.CallAssignInList> GetAssignmentsByCallId(int callId)
    {
        try
        {
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.blMutex)
                assignments = _dal.Assignment.ReadAll().Where(a => a.CallId == callId).ToList();

            return assignments.Select(a => new BO.CallAssignInList
            {
                VolunteerId = a.VolunteerId,
                FullName = Helpers.VolunteerManager.GetVolunteerFullName(a.VolunteerId),
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = (BO.AssignmentStatus?)a.Status
            });
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralException("Something went wrong while retrieving assignments for call.", ex);
        }
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
        CallManager.Observers.AddListObserver(listObserver);
    public void AddObserver(int id, Action observer) =>
        CallManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(Action listObserver) =>
        CallManager.Observers.RemoveListObserver(listObserver);
    public void RemoveObserver(int id, Action observer) =>
        CallManager.Observers.RemoveObserver(id, observer);
    #endregion Stage 5
}