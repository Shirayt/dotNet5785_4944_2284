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

    // Determines the status of each call based on the latest assignment.
    public static IEnumerable<(int CallId, CallStatus Status)> GetCallStatuses()
    {
        var calls = s_dal.Call.ReadAll();

        return calls.Select(call => GetCallStatus(call));
    }

    public static (int CallId, CallStatus Status) GetCallStatus(DO.Call call)
    {
        var calls = s_dal.Call.ReadAll();

        var assignment = s_dal.Assignment .ReadAll() .FirstOrDefault(a => a.CallId == call.Id);

        var assignmentStatus = assignment?.Status;

        // No assignment or assignment was canceled
        if (assignmentStatus == null ||
            assignmentStatus == DO.AssignmentStatus.SelfCancelled ||
            assignmentStatus == DO.AssignmentStatus.ManagerCancelled)
        {
            // Check if the call is at risk 
            if (call.MaxEndTime.HasValue && (call.MaxEndTime.Value - AdminManager.Now).TotalHours <= s_dal.Config.RiskRange.TotalHours)
            {
                return (call.Id, CallStatus.OpenAtRisk);
            }
            return (call.Id, CallStatus.Open);
        }

        // If the call is completed
        if (assignmentStatus == DO.AssignmentStatus.Completed)
        {
            return (call.Id, CallStatus.Closed);
        }

        // If the assignment expired
        if (assignmentStatus == DO.AssignmentStatus.Expired)
        {
            return (call.Id, CallStatus.Expired);
        }

        // The call is in processing
        return (call.Id, CallStatus.InProcessing);
    }

    public static TimeSpan? CalculateRestTimeForCall(DO.Call call)
    {
        var currentTime = AdminManager.Now;
        return currentTime > call.OpenTime ? currentTime - call.OpenTime : null;
    }

    public static string? GetLastVolunteerName(DO.Call call)
    {
        var assignments = s_dal.Assignment.ReadAll();
        var volunteers = s_dal.Volunteer.ReadAll();

        var lastAssignment = assignments.Where(a => a.CallId == call.Id).OrderByDescending(a => a.StartTime).FirstOrDefault();
        return lastAssignment != null ? volunteers.FirstOrDefault(v => v.Id == lastAssignment.VolunteerId)?.FullName : null;
    }

    public static TimeSpan? CalculateRestTimeForTreatment(DO.Call call)
    {
        var assignments = s_dal.Assignment.ReadAll();
        var completedAssignments = assignments.Where(a => a.CallId == call.Id && a.Status == DO.AssignmentStatus.Completed);
        return completedAssignments.Any() ? completedAssignments.Max(a => a.EndTime) - call.OpenTime : null;
    }
    public static int GetAllocationsAmount(int callId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        return assignments.Count(a => a.CallId == callId);
    }

    public static void ValidateBOCallData(BO.Call call)
    {
        // Check if call object is null
        if (call == null)
        {
            throw new BlArgumentNullException("BO Call object cannot be null.");
        }

        // Validate CallType is defined in the enum
        if (!Enum.IsDefined(typeof(BO.CallType), call.CallType))
        {
            throw new BlInvalidInputException("Invalid call type.");
        }

        // Validate Description (optional field)
        if (call.Description != null && call.Description.Length > 500)
        {
            throw new BlInvalidInputException("Description cannot exceed 500 characters.");
        }

        // Validate FullAddress (optional field)
        if (call.FullAddress != null && string.IsNullOrWhiteSpace(call.FullAddress))
        {
            throw new BlArgumentNullException("Full address cannot be empty or whitespace if provided.");
        }

        // Validate address and coordinates: non-empty address and valid coordinate ranges
        if (string.IsNullOrWhiteSpace(call.FullAddress) ||
            call.Latitude < -90 || call.Latitude > 90 ||
            call.Longitude < -180 || call.Longitude > 180)
        {
            throw new BlInvalidInputException("Invalid address or coordinates.");
        }

        // Validate OpenTime (cannot be in the future)
        if (call.OpenTime > AdminManager.Now)
        {
            throw new BlInvalidTimeException("Open time cannot be in the future.");
        }

        // Validate MaxEndTime (must be after OpenTime if provided)
        if (call.MaxEndTime.HasValue && call.MaxEndTime.Value <= call.OpenTime)
        {
            throw new BlInvalidTimeException("Maximum end time must be after open time.");
        }

        // Validate Status is defined in the enum
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
        // Filter by filterType if not null
        if (filterType != null)
        {
            var propertyInfo = typeof(T).GetProperty(filterType.ToString());
            if (propertyInfo != null)
                calls = calls.Where(c => propertyInfo.GetValue(c) != null);

        }

        // Sort by sortField if not null
        if (sortField != null)
        {
            var propertyInfo = typeof(T).GetProperty(sortField.ToString());
            if (propertyInfo != null)
                calls = calls.OrderBy(c => propertyInfo.GetValue(c));

        }
        else
        {
            // Default sort by Id
            var propertyInfo = typeof(T).GetProperty("Id");
            if (propertyInfo != null)
                calls = calls.OrderBy(c => propertyInfo.GetValue(c));

        }

        return calls;
    }
    public static double GetDistanceFromVolunteer(int volunteerId, DO.Call call)
    {
        var volunteer = s_dal.Volunteer.Read(volunteerId);

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

}

