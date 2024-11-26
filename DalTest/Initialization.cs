
namespace DalTest;
using DalApi;
using DO;
using System.Xml.Linq;

public static class Initialization
{
    private static ICall? s_dalCall;
    private static IAssignment? s_dalAssignment;
    private static IVolunteer? s_dalVolunteer;
    private static IConfig? s_dalConfig;

    private static readonly Random s_rand = new();

    public static void Initialize(ICall dalCall, IAssignment dalAssignment, IVolunteer dalVolunteer, IConfig dalConfig)
    {
        s_dalCall = dalCall;
        s_dalAssignment = dalAssignment;
        s_dalVolunteer = dalVolunteer;
        s_dalConfig = dalConfig;

        createVolunteer();
        createCall();
        createAssignment();
    }
    private static void createVolunteer()
    {
        string[] volunteerNames = { "Dani Levy", "Shira Cohen", "Tali Amar", "Yair Israelof", "Eli Klein", "Maya Shimon", "Avi Ben-David", "Liat Ron", "Yaara Berman", "Noa Sasson", "Roi Peretz", "Gal Zohar", "Dana Kfir", "Yosef Shaked", "Ruth Erez" };
        string[] emails = { "dani@gmail.com", "shira@gmail.com", "tali@gmail.com", "yair@gmail.com", "eli@gmail.com", "maya@gmail.com", "avi@gmail.com", "liat@gmail.com", "yaara@gmail.com", "noa@gmail.com", "roi@gmail.com", "gal@gmail.com", "dana@gmail.com", "yosef@gmail.com", "ruth@gmail.com" };
        string[] phones = { "0541111111", "0542222222", "0543333333", "0544444444", "0545555555", "0546666666", "0547777777", "0548888888", "0549999999", "0550000000", "0551111111", "0552222222", "0553333333", "0554444444", "0555555555" };

        for (int i = 0; i < volunteerNames.Length; i++)
        {
            int id;
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dalVolunteer!.Read(id) != null);

            // הגרלת המרחק המרבי לקבלת קריאה בטווח של 5 עד 100
            double maxDistanceForCall = s_rand.Next(5, 101);

            Volunteer volunteer = new()
            {
                Id = id,
                FullName = volunteerNames[i],
                Email = emails[i],
                PhoneNumber = phones[i],
                MaxDistanceForCall = maxDistanceForCall,
                DistanceType = DistanceType.Air
            };
            s_dalVolunteer!.Create(volunteer);
        }
    }


    private static void createCall()
    {
        string[] descriptions = { "Emergency A", "Emergency B", "Technical C", "Fire D", "Medical E" };

        for (int i = 0; i < 50; i++)
        {
            int id = s_dalConfig!.NextCallId;
            DateTime openTime = s_dalConfig.Clock.AddDays(-s_rand.Next(1, 100));
            DateTime? closeTime = i % 10 == 0 ? null : openTime.AddHours(s_rand.Next(1, 72));

            Call call = new()
            {
                Id = id,
                Description = descriptions[i % descriptions.Length],
                OpenTime = openTime,
                CloseTime = closeTime
            };
            s_dalCall!.Create(call);
        }
    }

    //private static void createCall()
    //{
    //    string[] descriptions =
    //    {
    //    "Emergency: Urgent medical help required",
    //    "Emergency: Fire hazard in remote location",
    //    "Equipment: Need medical equipment",
    //    "Doctor: Urgent need for a doctor",
    //    "Training: Request for volunteer medical training"
    //};

    //    Random rand = new Random();
    //    DateTime now = s_dalConfig!.Clock;

    //    // יצירת 50 קריאות
    //    for (int i = 0; i < 50; i++)
    //    {
    //        int id = s_dalConfig!.NextCallId;
    //        DateTime openTime = now.AddDays(-rand.Next(1, 100));
    //        CallType type = (CallType)rand.Next(0, 4);
    //        DateTime? closeTime = rand.NextDouble() < 0.8 ? openTime.AddHours(rand.Next(1, 72)) : (DateTime?)null;

    //        Call call = new()
    //        {
    //            Id = id,
    //            CallType = type,
    //            Description = descriptions[(int)type],
    //            FullAddress = "Remote Location Address",
    //            Latitude = rand.NextDouble() * 180 - 90,
    //            Longitude = rand.NextDouble() * 360 - 180,
    //            OpenTime = openTime,
    //            MaxEndTime = closeTime
    //        };

    //        s_dalCall!.Create(call);
    //    }

    //    // יצירת 15 קריאות שלא הוקצו כלל
    //    for (int i = 0; i < 15; i++)
    //    {
    //        int id = s_dalConfig!.NextCallId;
    //        DateTime openTime = now.AddDays(-rand.Next(1, 100));

    //        Call call = new()
    //        {
    //            Id = id,
    //            CallType = (CallType)rand.Next(0, 4),
    //            Description = descriptions[rand.Next(descriptions.Length)],
    //            FullAddress = "Remote Location Address",
    //            Latitude = rand.NextDouble() * 180 - 90,
    //            Longitude = rand.NextDouble() * 360 - 180,
    //            OpenTime = openTime,
    //            MaxEndTime = null
    //        };

    //        s_dalCall!.Create(call);
    //    }

    //    // יצירת 5 קריאות שפג תוקפן
    //    for (int i = 0; i < 5; i++)
    //    {
    //        int id = s_dalConfig!.NextCallId;
    //        DateTime openTime = now.AddDays(-rand.Next(100, 200));

    //        Call call = new()
    //        {
    //            Id = id,
    //            CallType = (CallType)rand.Next(0, 4),
    //            Description = descriptions[rand.Next(descriptions.Length)],
    //            FullAddress = "Remote Location Address",
    //            Latitude = rand.NextDouble() * 180 - 90,
    //            Longitude = rand.NextDouble() * 360 - 180,
    //            OpenTime = openTime,
    //            MaxEndTime = now.AddDays(-rand.Next(1, 5))
    //        };

    //        s_dalCall!.Create(call);
    //    }
    //}


    private static void createAssignment()
    {
        List<Volunteer?> volunteers = s_dalVolunteer!.GetAll().ToList();
        List<Call?> calls = s_dalCall!.GetAll().ToList();

        for (int i = 0; i < 50; i++)
        {
            int id = s_dalConfig!.NextAssignmentId;
            Volunteer volunteer = volunteers[s_rand.Next(volunteers.Count)]!;
            Call call = calls[s_rand.Next(calls.Count)]!;

            Assignment assignment = new()
            {
                Id = id,
                VolunteerId = volunteer.Id,
                CallId = call.Id,
                StartTime = call.OpenTime.AddMinutes(s_rand.Next(1, 120)),
                EndTime = call.CloseTime?.AddMinutes(-s_rand.Next(1, 120))
            };
            s_dalAssignment!.Create(assignment);
        }
    }
}

