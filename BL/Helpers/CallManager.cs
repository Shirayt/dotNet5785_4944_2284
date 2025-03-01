using BO;
using DalApi;
using DO;
namespace Helpers;

/// <summary>
/// Internal static helper class for all call data
/// </summary>
internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    // Determines the status of each call based on the latest assignment.
    public static IEnumerable<(int CallId, CallStatus Status)> GetCallStatuses()
    {
        var calls = s_dal.Call.ReadAll();

        return calls.Select(call => GetCallStatus(call));
    }

    public static (int CallId, CallStatus Status) GetCallStatus(DO.Call call)
    {
        var calls = s_dal.Call.ReadAll();
        var assignments = s_dal.Assignment.ReadAll();

        var callAssignments = assignments.Where(a => a.CallId == call.Id);
        var latestAssignment = callAssignments.OrderByDescending(a => a.StartTime).FirstOrDefault();
        var latestAssignmentStatus = latestAssignment?.Status;

        // No assignment or assignment was canceled
        if (latestAssignmentStatus == null ||
            latestAssignmentStatus == DO.AssignmentStatus.SelfCancelled ||
            latestAssignmentStatus == DO.AssignmentStatus.ManagerCancelled)
        {
            // Check if the call is at risk
            if (call.MaxEndTime.HasValue && (call.MaxEndTime.Value - DateTime.Now).TotalHours <= 50)
            {
                return (call.Id, CallStatus.OpenAtRisk);
            }
            return (call.Id, CallStatus.Open);
        }

        // If the call is completed
        if (latestAssignmentStatus == DO.AssignmentStatus.Completed)
        {
            return (call.Id, CallStatus.Closed);
        }

        // If the assignment expired
        if (latestAssignmentStatus == DO.AssignmentStatus.Expired)
        {
            return (call.Id, CallStatus.Expired);
        }

        // The call is in processing
        return (call.Id, CallStatus.InProcessing);
    }

    public static TimeSpan? CalculateRestTimeForCall(DO.Call call)
    {
        var currentTime = DateTime.Now;
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


}
