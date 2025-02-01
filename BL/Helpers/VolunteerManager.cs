﻿using DalApi;
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
            throw new Exception("Invalid email format.");
        }

        // Validate phone number: must be numeric and at least 10 digits
        if (!volunteer.PhoneNumber.All(char.IsDigit) || volunteer.PhoneNumber.Length < 10)
        {
            throw new Exception("Invalid phone number.");
        }

        // Validate the ID: must be positive
        if (volunteer.Id <= 0 || volunteer.Id.ToString().Length != 9 ||
            volunteer.Id.ToString().Select((c, i) => (c - '0') * (i % 2 == 0 ? 1 : 2)).Sum() % 10 != 0)
        {
            throw new Exception("Invalid ID.");
        }

        // Validate address and coordinates: non-empty address and valid coordinate ranges
        if (string.IsNullOrWhiteSpace(volunteer.CurrentFullAddress) ||
            volunteer.Latitude < -90 || volunteer.Latitude > 90 ||
            volunteer.Longitude < -180 || volunteer.Longitude > 180)
        {
            throw new Exception("Invalid address or coordinates.");
        }

        // Validates that the input value is a defined DistanceType.
        if (!Enum.IsDefined(typeof(DO.DistanceType), volunteer.DistanceType))
        {
            throw new ArgumentException("Invalid DistanceType value.");
        }


    }

}



//    internal static BO.Year GetStudentCurrentYear(DateTime? registrationDate)
//    {
//        BO.Year currYear = (BO.Year)(ClockManager.Now.Year - registrationDate?.Year!);
//        return currYear > BO.Year.None ? BO.Year.None : currYear;
//    }

//    internal static BO.StudentInCourse GetDetailedCourseForStudent(int studentId, int courseId)
//    {
//        DO.Link? doLink = s_dal.Link.Read(l => l.StudentId == studentId && l.CourseId == courseId)
//        ?? throw new BO.BlDoesNotExistException($"Student with ID={studentId} does Not take Course
//        with ID ={ courseId }");
//DO.Course? doCourse = s_dal.Course.Read(courseId)
//?? throw new BO.BlDoesNotExistException($"Course with ID={courseId} does Not exist");
//        return new()
//        {
//            StudentId = studentId,
//            Course = new Tuple<int, string, string>(doCourse.Id, doCourse.CourseNumber,
//        doCourse.CourseName),
//            InYear = (BO.Year?)doCourse.InYear,
//            InSemester = (BO.SemesterNames?)doCourse.InSemester,
//            Grade = doLink.Grade,
//            Credits = doCourse.Credits
//        };
//    }

//    internal static void PeriodicStudentsUpdates(DateTime oldClock, DateTime newClock) //stage 4
//    {
//        var list = s_dal.Student.ReadAll().ToList();
//        foreach (var doStudent in list)
//        {
//            //if student study for more than MaxRange years
//            //then student should be automatically updated to 'not active'
//            if (ClockManager.Now.Year - doStudent.RegistrationDate?.Year >= s_dal.Config.MaxRange)
//            {
//                s_dal.Student.Update(doStudent with { IsActive = false });
//            }
//        }
//    }
