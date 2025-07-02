using BO;
using DalApi;
using DO;
using System;
using System.Text.RegularExpressions;
namespace Helpers;

/// <summary>
/// Internal static helper class for all call data
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 

    public static IEnumerable<(int CallId, CallStatus Status)> GetCallStatuses()
    {
        IEnumerable<DO.Call> calls;
        lock (AdminManager.blMutex)
        {
            calls = s_dal.Call.ReadAll();
        }

        return calls.Select(call => GetCallStatus(call));
    }

    public static (int CallId, CallStatus Status) GetCallStatus(DO.Call call)
    {
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.blMutex)
        {
            assignments = s_dal.Assignment.ReadAll();
        }

        var assignment = assignments.OrderByDescending(a => a.Id).FirstOrDefault(a => a.CallId == call.Id);
        var assignmentStatus = assignment?.Status;

        TimeSpan? riskRange;
        lock (AdminManager.blMutex)
        {
            riskRange = s_dal.Config.RiskRange;
        }

        if (assignmentStatus == DO.AssignmentStatus.SelfCancelled ||
            assignmentStatus == DO.AssignmentStatus.ManagerCancelled)
        {
            if (call.MaxEndTime.HasValue && (call.MaxEndTime.Value - AdminManager.Now).TotalHours <= riskRange.Value.TotalHours)
            {
                return (call.Id, CallStatus.OpenAtRisk);
            }
            return (call.Id, CallStatus.Open);
        }

        if (assignmentStatus == DO.AssignmentStatus.Completed)
        {
            return (call.Id, CallStatus.Closed);
        }

        if (assignmentStatus == DO.AssignmentStatus.Expired)
        {
            return (call.Id, CallStatus.Expired);
        }

        return (call.Id, CallStatus.InProcessing);
    }

    public static TimeSpan? CalculateRestTimeForCall(DO.Call call)
    {
        var currentTime = AdminManager.Now;
        return currentTime > call.OpenTime ? currentTime - call.OpenTime : null;
    }

    public static string? GetLastVolunteerName(DO.Call call)
    {
        IEnumerable<DO.Assignment> assignments;
        IEnumerable<DO.Volunteer> volunteers;

        lock (AdminManager.blMutex)
        {
            assignments = s_dal.Assignment.ReadAll();
        }
        lock (AdminManager.blMutex)
        {
            volunteers = s_dal.Volunteer.ReadAll();
        }

        var lastAssignment = assignments.Where(a => a.CallId == call.Id).OrderByDescending(a => a.StartTime).FirstOrDefault();
        return lastAssignment != null ? volunteers.FirstOrDefault(v => v.Id == lastAssignment.VolunteerId)?.FullName : null;
    }

    public static TimeSpan? CalculateRestTimeForTreatment(DO.Call call)
    {
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.blMutex)
        {
            assignments = s_dal.Assignment.ReadAll();
        }

        var completedAssignments = assignments.Where(a => a.CallId == call.Id && a.Status == DO.AssignmentStatus.Completed);
        return completedAssignments.Any() ? completedAssignments.Max(a => a.EndTime) - call.OpenTime : null;
    }

    public static int GetAllocationsAmount(int callId)
    {
        IEnumerable<DO.Assignment> assignments;
        lock (AdminManager.blMutex)
        {
            assignments = s_dal.Assignment.ReadAll();
        }

        return assignments.Count(a => a.CallId == callId);
    }

    public static void ValidateBOCallData(BO.Call call)
    {
        if (call == null)
        {
            throw new BlArgumentNullException("BO Call object cannot be null.");
        }

        if (!Enum.IsDefined(typeof(BO.CallType), call.CallType))
        {
            throw new BlInvalidInputException("Invalid call type.");
        }

        if (call.Description != null && call.Description.Length > 500)
        {
            throw new BlInvalidInputException("Description cannot exceed 500 characters.");
        }

        if (call.FullAddress != null && string.IsNullOrWhiteSpace(call.FullAddress))
        {
            throw new BlArgumentNullException("Full address cannot be empty or whitespace if provided.");
        }

        if (string.IsNullOrWhiteSpace(call.FullAddress) ||
            call.Latitude < -90 || call.Latitude > 90 ||
            call.Longitude < -180 || call.Longitude > 180)
        {
            throw new BlInvalidInputException("Invalid address or coordinates.");
        }

        if (call.OpenTime > AdminManager.Now)
        {
            throw new BlInvalidTimeException("Open time cannot be in the future.");
        }

        if (call.MaxEndTime.HasValue && call.MaxEndTime.Value <= call.OpenTime)
        {
            throw new BlInvalidTimeException("Maximum end time must be after open time.");
        }

        if (!Enum.IsDefined(typeof(BO.CallStatus), call.Status))
        {
            throw new BlInvalidInputException("Invalid call status.");
        }
    }

    public static DO.Assignment GetAssignmentForCall(IEnumerable<Assignment> volunteerassignments, int callId)
    {
        return volunteerassignments.FirstOrDefault(a => a.CallId == callId);
    }

    public static IEnumerable<T> ApplyFilterAndSort<T>(IQueryable<T> calls, BO.FilterAndSortByFields? filterType, BO.FilterAndSortByFields? sortField)
    {
        if (filterType != null)
        {
            var propertyInfo = typeof(T).GetProperty(filterType.ToString());
            if (propertyInfo != null)
                calls = calls.Where(c => propertyInfo.GetValue(c) != null);
        }

        if (sortField != null)
        {
            var propertyInfo = typeof(T).GetProperty(sortField.ToString());
            if (propertyInfo != null)
                calls = calls.OrderBy(c => propertyInfo.GetValue(c));
        }
        else
        {
            var propertyInfo = typeof(T).GetProperty("Id");
            if (propertyInfo != null)
                calls = calls.OrderBy(c => propertyInfo.GetValue(c));
        }

        return calls;
    }

    public static double GetDistanceFromVolunteer(int volunteerId, DO.Call call)
    {
        DO.Volunteer volunteer;
        lock (AdminManager.blMutex)
        {
            volunteer = s_dal.Volunteer.Read(volunteerId);
        }

        if (volunteer.Latitude == null || volunteer.Longitude == null)
            throw new BlInvalidOperationException("Volunteer location is not available");

        const double EarthRadiusKm = 6371;
        double volunteerLat = volunteer.Latitude.Value;
        double volunteerLon = volunteer.Longitude.Value;
        double callLat = call.Latitude;
        double callLon = call.Longitude;

        double dLat = (callLat - volunteerLat) * (Math.PI / 180);
        double dLon = (callLon - volunteerLon) * (Math.PI / 180);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(volunteerLat * (Math.PI / 180)) * Math.Cos(callLat * (Math.PI / 180)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    public static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        var dal = Factory.Get;
        var callsToUpdate = dal.Call.ReadAll()
            .Where(c => c.MaxEndTime.HasValue && c.MaxEndTime <= newClock)
            .ToList();

        foreach (var call in callsToUpdate)
        {
            var assignments = dal.Assignment.ReadAll()
                .Where(a => a.CallId == call.Id && a.EndTime == null)
                .OrderByDescending(a => a.Id).ToList();

            var latestAssignment = assignments.FirstOrDefault();
            if (latestAssignment is null) continue;

            var updatedAssignment = latestAssignment with
            {
                Status = (DO.AssignmentStatus)BO.AssignmentStatus.Expired,
                EndTime = newClock
            };

            lock (AdminManager.blMutex)
                dal.Assignment.Update(updatedAssignment);
        }

        foreach (var call in callsToUpdate)
            Observers.NotifyItemUpdated(call.Id);

        Observers.NotifyListUpdated();
    }
}
