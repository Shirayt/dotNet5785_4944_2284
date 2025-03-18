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
    private static IDal s_dal = Factory.Get; //stage 4


    public static int GetCompletedCallsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        return assignments.Count(a => a.VolunteerId == volunteerId && a.Status == DO.AssignmentStatus.Completed);
    }
    public static int GetSelfCancelledCallsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        return assignments.Count(a => a.VolunteerId == volunteerId && a.Status == DO.AssignmentStatus.SelfCancelled);
    }
    public static int GetExpiredCallsCount(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        return assignments.Count(a => a.VolunteerId == volunteerId && a.Status == DO.AssignmentStatus.Expired);
    }

    public static int? GetCallInTreatment(int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        return assignments
            .Where(a => a.VolunteerId == volunteerId && a.EndTime == null)
            .Select(a => (int?)a.CallId)
            .FirstOrDefault();
    }

    public static void ValidateBOVolunteerData(BO.Volunteer volunteer)
    {
        // Validate email format
        var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // Simple email pattern
        if (!Regex.IsMatch(volunteer.Email, emailPattern))
        {
            throw new BlFormatException("Invalid email format.");
        }

        // Validate phone number: must be numeric and at least 10 digits
        if (!volunteer.PhoneNumber.All(char.IsDigit) || volunteer.PhoneNumber.Length < 10)
        {
            throw new BlInvalidInputException("Invalid phone number.");
        }

        // Validate the ID: must be positive
        if (volunteer.Id <= 0 || volunteer.Id.ToString().Length != 9 ||
            volunteer.Id.ToString().Select((c, i) => (c - '0') * (i % 2 == 0 ? 1 : 2)).Sum() % 10 != 0)
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
        if (!Enum.IsDefined(typeof(DO.DistanceType), volunteer.DistanceType))
        {
            throw new BlInvalidInputException("Invalid DistanceType value.");
        }
    }
    public static BO.CallInProgress? GetCallInProgress( int volunteerId)
    {
        var assignments = s_dal.Assignment.ReadAll();
        var calls =s_dal.Call.ReadAll();

        return assignments
            .Where(a => a.VolunteerId == volunteerId && a.EndTime == null)
            .Join(calls, a => a.CallId, c => c.Id, (a, c) => new BO.CallInProgress
            {
                CallId = c.Id,
                AssignmentId = a.Id,
                CallType = (BO.CallType)c.CallType,
                Description = c.Description,
                FullAddress = c.FullAddress,
                OpenTime = a.StartTime,
                MaxEndTime = c.MaxEndTime,
                TreatmentStartTime = a.StartTime,
                DistanceFromVolunteer = 0,
                Status = (a.Status == null || a.Status == DO.AssignmentStatus.Completed)
                    ? BO.StatusCallInProgress.InProcessing
                    : BO.StatusCallInProgress.InProcessingInRisk
            })
            .FirstOrDefault();
    }
}
