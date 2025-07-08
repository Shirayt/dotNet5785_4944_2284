using BlApi;
using BlImplementation;
using BO;
using DalApi;
using DO;
using System.Text.RegularExpressions;
namespace Helpers;

/// <summary>
/// Internal static helper class for all volunteer data
/// </summary>
internal static class VolunteerManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4
    private static IBl s_bl = BlApi.Factory.Get();
    internal static ObserverManager Observers = new(); //stage 5 


    private static readonly Random s_rand = new();
    private static int s_simulatorCounter = 0;


    private static int threadIndex = 0;
    internal static void SimulateVolunteerAssignmentsAndCallHandling()
    {
        Thread.CurrentThread.Name = $"Simulator{threadIndex++}";

        List<int> updatedVolunteerIds = new();
        List<int> updatedCallIds = new();

        List<DO.Volunteer> activeVolunteers;
        lock (AdminManager.blMutex)
        {
            activeVolunteers = DalApi.Factory.Get.Volunteer.ReadAll(v => v.IsActive).ToList();
        }

        foreach (var volunteer in activeVolunteers)
        {
            DO.Assignment? currentAssignment;
            lock (AdminManager.blMutex)
            {
                currentAssignment = DalApi.Factory.Get.Assignment
                    .ReadAll(a => a.VolunteerId == volunteer.Id && a.EndTime == null)
                    .FirstOrDefault();
            }

            if (currentAssignment == null)
            {
                List<BO.OpenCallInList> openCalls = s_bl.Call.GetOpenCallsByVolunteer(volunteer.Id, null, null).ToList();

                if (!openCalls.Any() || Random.Shared.NextDouble() > 0.5) continue;

                var selectedCall = openCalls[Random.Shared.Next(openCalls.Count)];
                try
                {
                    s_bl.Call.SelectCallForTreatment(volunteer.Id, selectedCall.Id);
                    updatedVolunteerIds.Add(volunteer.Id);
                    updatedCallIds.Add(selectedCall.Id);
                }
                catch { continue; }
            }
            else
            {
                DO.Call? call;
                lock (AdminManager.blMutex)
                {
                    call = DalApi.Factory.Get.Call.Read(c => c.Id == currentAssignment.CallId);
                }

                if (call is null) continue;

                double distance = Tools.CalculateDistance(volunteer.Latitude!, volunteer.Longitude!, call.Latitude, call.Longitude);
                TimeSpan baseTime = TimeSpan.FromMinutes(distance * 2);
                TimeSpan extra = TimeSpan.FromMinutes(Random.Shared.Next(1, 5));
                TimeSpan totalNeeded = baseTime + extra;
                TimeSpan? actual = AdminManager.Now - currentAssignment.StartTime;

                double r = Random.Shared.NextDouble();
                if (r < 0.4)
                {
                    try
                    {
                        s_bl.Call.CancelCallAssignment(volunteer.Id, currentAssignment.Id);
                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(call.Id);
                    }
                    catch { continue; }
                }
                else if (r < 0.8)
                {
                    try
                    {
                        s_bl.Call.MarkCallAsCompleted(volunteer.Id, currentAssignment.Id);
                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(call.Id);
                    }
                    catch { continue; }
                }

            }
        }

        foreach (var id in updatedVolunteerIds.Distinct())
            VolunteerManager.Observers.NotifyItemUpdated(id);

        foreach (var id in updatedCallIds.Distinct())
            CallManager.Observers.NotifyItemUpdated(id);

        VolunteerManager.Observers.NotifyListUpdated();
    }

    public static int GetCompletedCallsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        if (assignments is null) return 0;
        return assignments.Count(a => a.VolunteerId == volunteerId && a.Status == DO.AssignmentStatus.Completed);
    }
    public static int GetSelfCancelledCallsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        if (assignments is null) return 0;
        return assignments.Count(a => a.VolunteerId == volunteerId && a.Status == DO.AssignmentStatus.SelfCancelled);
    }
    public static int GetExpiredCallsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        if (assignments is null) return 0;
        return assignments.Count(a => a.VolunteerId == volunteerId && a.Status == DO.AssignmentStatus.Expired);
    }

    public static int? GetCallInTreatment(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        if (assignments is null) return 0;
        return assignments
            .Where(a => a.VolunteerId == volunteerId && a.EndTime == null)
            .Select(a => (int?)a.CallId)
            .FirstOrDefault();
    }

    public static void ValidateBOVolunteerData(BO.Volunteer volunteer)
    {
        // Validate email format
        var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";// Simple email pattern
        if (!Regex.IsMatch(volunteer.Email, emailPattern))
        {
            throw new BlFormatException("Invalid email format.");
        }

        // Validate phone number: must be numeric and at least 10 digits
        if (!volunteer.PhoneNumber.All(char.IsDigit) || volunteer.PhoneNumber.Length < 10)
        {
            throw new BlInvalidInputException("Invalid phone number.");
        }

        if (volunteer.Id <= 0 || volunteer.Id.ToString().PadLeft(9, '0').Length != 9 ||
    volunteer.Id.ToString().PadLeft(9, '0').Reverse().Select((c, i) =>
    {
        int digit = c - '0';
        int multiplied = digit * (i % 2 == 0 ? 1 : 2);
        return multiplied > 9 ? multiplied - 9 : multiplied;
    }).Sum() % 10 != 0)
        {
            throw new BlInvalidInputException("Invalid ID.");
        }

        // Validate address and coordinates: non-empty address and valid coordinate ranges
        if (string.IsNullOrWhiteSpace(volunteer.CurrentFullAddress) ||
            volunteer.Latitude < -90 || volunteer.Latitude > 90 ||
            volunteer.Longitude < -180 || volunteer.Longitude > 180)
        {
            throw new BlInvalidInputException("Invalid address or coordinates.");
        }

        // Validates that the input value is a defined DistanceType.
        if (!Enum.IsDefined(typeof(BO.DistanceType), volunteer.DistanceType))
        {
            throw new BlInvalidInputException("Invalid DistanceType value.");
        }
    }

    public static bool IsSingleManager(int volunteerId, DO.Role selfRole)
    {
       IEnumerable<DO.Volunteer> ? volunteers;

        lock (AdminManager.blMutex)
        {
            volunteers = s_dal.Volunteer.ReadAll();
        }

        return volunteers.Count(v => v.Role == DO.Role.Manager) == 1 && selfRole == DO.Role.Manager;
    }

    public static BO.CallInProgress? GetCallInProgress(int volunteerId)
    {
        var volunteer = s_dal.Volunteer.Read(volunteerId);
        var assignments = s_dal.Assignment.ReadAll()?.ToList();
        var calls = s_dal.Call.ReadAll()?.ToList();

        var openAssignments = assignments
            .Where(a =>
                a.VolunteerId == volunteerId &&
                (a.EndTime == null || a.EndTime == DateTime.MinValue) &&
                a.Status == null)
            .ToList();

        if (!openAssignments.Any())
            return null;

        var assignment = openAssignments.First();

        var call = calls.FirstOrDefault(c => c.Id == assignment.CallId) ?? throw new Exception($"No call found with ID {assignment.CallId}.");

        return new BO.CallInProgress
        {
            CallId = call.Id,
            AssignmentId = assignment.Id,
            CallType = (BO.CallType)call.CallType,
            Description = call.Description,
            FullAddress = call.FullAddress,
            OpenTime = assignment.StartTime,
            MaxEndTime = call.MaxEndTime,
            TreatmentStartTime = assignment.StartTime,
            DistanceFromVolunteer = Tools.CalculateDistance(volunteer.Latitude, volunteer.Longitude, call.Latitude, call.Longitude),
            Status = BO.StatusCallInProgress.InProcessing
        };
    }


    public static string? GetVolunteerFullName(int volunteerId)
    {
        return s_dal.Volunteer.Read(volunteerId).FullName;
    }

    public static void PeriodicVolunteerUpdates(DateTime oldClock, DateTime newClock)
    {
        var assignments = s_dal.Assignment.ReadAll() ?? Enumerable.Empty<DO.Assignment>();
        var udatedAssignments = assignments
            .Where(a => a.EndTime == null)
            .ToList();

        var toUpdate = udatedAssignments
            .Select(a => new { Assignment = a, Call = s_dal.Call.Read(c => c.Id == a.CallId) })
            .Where(ac => ac.Call.MaxEndTime.HasValue && ac.Call.MaxEndTime <= newClock)
            .ToList();

        foreach (var ac in toUpdate)
        {
            var updatedAssignment = ac.Assignment with
            {
                Status = (DO.AssignmentStatus)BO.AssignmentStatus.Expired,
                EndTime = newClock
            };

            lock (AdminManager.blMutex)
                s_dal.Assignment.Update(updatedAssignment);
        }

        foreach (var ac in toUpdate.Select(a => a.Assignment))
            Observers.NotifyItemUpdated(ac.VolunteerId);

        Observers.NotifyListUpdated();
    }
}

