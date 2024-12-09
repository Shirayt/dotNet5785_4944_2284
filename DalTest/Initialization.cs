
namespace DalTest;
using DalApi;
using DO;
using System.Linq;

public static class Initialization
{
    private static IDal? s_dal;
    private static readonly Random s_rand = new();

    public static void Do(IDal dal) //stage 2
    {
        try
        {
            s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2
        }
        catch (NullReferenceException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal!.ResetDB();//stage 2

        createVolunteer();
        createCall();
        createAssignment();
    }
    private static void createVolunteer()
    {
        string[] volunteerNames = { "Dani Levy", "Shira Cohen", "Tali Amar", "Yair Israelof", "Eli Klein", "Maya Shimon", "Avi Ben-David", "Liat Ron", "Yaara Berman", "Noa Sasson", "Roi Peretz", "Gal Zohar", "Dana Kfir", "Yosef Shaked", "Ruth Erez" };
        string[] emails = { "dani@gmail.com", "shira@gmail.com", "tali@gmail.com", "yair@gmail.com", "eli@gmail.com", "maya@gmail.com", "avi@gmail.com", "liat@gmail.com", "yaara@gmail.com", "noa@gmail.com", "roi@gmail.com", "gal@gmail.com", "dana@gmail.com", "yosef@gmail.com", "ruth@gmail.com" };
        string[] phones = { "0541111111", "0542222222", "0543333333", "0544444444", "0545555555", "0546666666", "0547777777", "0548888888", "0549999999", "0550000000", "0551111111", "0552222222", "0553333333", "0554444444", "0555555555" };
        string[] addresses = { "29 Chinggis Avenue, Ulaanbaatar, Mongolia", "2 Tindall Street, Nelson, New Zealand", "Via Lago di Bracciano 32, 00069 Trevignano Romano, Italy", "145 Kulusuk, Greenland", "Vorshikhinskaya Street 11, Yakutsk, Russia", "Calle de los Reyes Católicos, 23, Fuerteventura, Spain", "9 Ngong Road, Nairobi, Kenya", "Bramber, West Sussex, England, United Kingdom", "Station Road, Tindivanam, Tamil Nadu, India", "Boko Haram Village, Borno State, Nigeria", "Lima Norte, Lima, Peru", "Pico de Orizaba, Veracruz, Mexico", "Desolation Sound, British Columbia, Canada", "Toraja, Sulawesi, Indonesia", "Mount Roraima, Venezuela, Brazil, Guyana" };
        double[] latitudes = { 47.8862, -40.5170, 42.1350, 65.6233, 62.0360, 28.4320, -1.2864, 50.8610, 12.2069, 12.1776, -12.0450, 19.0299, 57.3000, 2.4769, 6.7350 };
        double[] longitudes = { 106.9057, 173.2885, 12.2715, -37.4594, 129.7327, -13.8597, 36.8250, -0.3865, -0.2976, 13.4918, -77.2749, -97.2071, -60.8103, -60.6933, -60.7371 };
        try
        {
            for (int i = 0; i < volunteerNames.Length; i++)
            {
                int id;

                do
                {
                    id = s_rand.Next(200000000, 400000000);
                    //} while (s_dalVolunteer!.Read(id) != null);//stage 1
                } while (s_dal!.Volunteer.Read(id) != null);//stage 2

                // הגרלת המרחק המרבי לקבלת קריאה בטווח של 5 עד 100
                double maxDistanceForCall = s_rand.Next(5, 101);

                // יצירת המתנדב
                Volunteer volunteer = new Volunteer(
                    id,                                // ערך עבור Id
                    volunteerNames[i],                 // ערך עבור FullName
                    phones[i],                         // ערך עבור PhoneNumber
                    emails[i],                         // ערך עבור Email
                    addresses[i],                      // ערך עבור CurrentFullAddress
                    latitudes[i],                      // ערך עבור Latitude
                    longitudes[i],                     // ערך עבור Longitude
                    Role.Volunteer,                    // ערך עבור Role
                    true,                              // ערך עבור IsActive
                    maxDistanceForCall,                // ערך עבור MaxDistanceForCall
                    DistanceType.Air                   // ערך עבור DistanceType
                );
                // הוספת המתנדב למערכת
                s_dal!.Volunteer.Create(volunteer);
            }
        }
        catch (DalAlreadyExistsException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    private static void createCall()
    {
        // יצירת לפחות 50 קריאות
        int allocatedCalls = 0;  // סופר קריאות שהוקצו
        int unallocatedCalls = 0;  // סופר קריאות שלא הוקצו
        int expiredCalls = 0;  // סופר קריאות שפג תוקפן

        string[] descriptions =
        {
            "Emergency: Urgent medical help required",
            "Equipment: Need medical equipment for MCI",
            "Doctor: Urgent need for a doctor",
            "Training: Request for volunteer medical training"
            };

        var locations = new[] {
           new { Latitude = -59.524, Longitude = -43.024, FullAddress = "Tristan da Cunha, South Atlantic Ocean" },
           new { Latitude = -37.070, Longitude = 178.020, FullAddress = "Bouvet Island, South Atlantic Ocean" },
           new { Latitude = -24.396308, Longitude = 123.213434, FullAddress = "Christmas Island, Indian Ocean" },
           new { Latitude = 77.560000, Longitude = -104.157000, FullAddress = "Alert, Canada" },
           new { Latitude = -33.506, Longitude = 137.474, FullAddress = "Macquarie Island, Southern Ocean" },
           new { Latitude = 51.961, Longitude = -57.641, FullAddress = "Saint Pierre and Miquelon, North Atlantic Ocean" },
           new { Latitude = 30.3753, Longitude = 78.4744, FullAddress = "India (remote region)" },
           new { Latitude = 4.8751, Longitude = 7.5251, FullAddress = "Ogoniland, Nigeria" },
           new { Latitude = -62.957, Longitude = 139.063, FullAddress = "South Georgia and the South Sandwich Islands" },
           new { Latitude = -22.957, Longitude = 142.731, FullAddress = "Oceania (remote island)" },
           new { Latitude = -15.3601, Longitude = 135.885, FullAddress = "Northern Australia (remote area)" },
           new { Latitude = -63.3461, Longitude = -60.6791, FullAddress = "Falkland Islands" },
           new { Latitude = 47.1243, Longitude = -122.7772, FullAddress = "Diomede Islands, Bering Strait" },
           new { Latitude = 25.0608, Longitude = -77.1303, FullAddress = "Bimini, Bahamas" },
           new { Latitude = -12.3456, Longitude = 40.9876, FullAddress = "Comoros Islands" },
           new { Latitude = -36.9379, Longitude = -63.9000, FullAddress = "Patagonia, Argentina" },
           new { Latitude = 50.755, Longitude = 16.432, FullAddress = "Czech Republic, remote area" },
           new { Latitude = -13.518, Longitude = 142.945, FullAddress = "Cape York Peninsula, Australia" },
           new { Latitude = 69.344, Longitude = 39.822, FullAddress = "Central Siberia, Russia" },
           new { Latitude = 66.5648, Longitude = 90.9623, FullAddress = "Severnaya Zemlya, Russia" },
           new { Latitude = 1.206, Longitude = 78.713, FullAddress = "Andaman Islands, India" },
           new { Latitude = 6.6670, Longitude = 100.0, FullAddress = "Anambas Islands, Malaysia" },
           new { Latitude = 9.0720, Longitude = 78.6651, FullAddress = "Chilika Lake, India" },
           new { Latitude = -50.448, Longitude = 101.577, FullAddress = "Kerguelen Islands, Southern Ocean" },
           new { Latitude = 42.659, Longitude = -47.097, FullAddress = "Ponta do Ouro, Mozambique" },
           new { Latitude = -21.100, Longitude = -70.300, FullAddress = "Juan Fernández Islands, Chile" },
           new { Latitude = 34.694, Longitude = -83.235, FullAddress = "Chattahoochee-Oconee National Forest, Georgia, USA" },
           new { Latitude = 29.4997, Longitude = -97.7467, FullAddress = "Dinosaur Valley State Park, Texas, USA" },
           new { Latitude = 36.405, Longitude = 18.677, FullAddress = "Algerian Desert, Algeria" },
           new { Latitude = 45.832, Longitude = -98.579, FullAddress = "South Dakota, USA" },
           new { Latitude = -11.629, Longitude = 149.035, FullAddress = "Vanuatu Islands, Pacific Ocean" },
           new { Latitude = 56.794, Longitude = -106.725, FullAddress = "Saskatchewan, Canada" },
           new { Latitude = 52.0000, Longitude = 172.0000, FullAddress = "Pacific Ocean, Near Kamchatka, Russia" },
           new { Latitude = 32.654, Longitude = -69.897, FullAddress = "Bermuda Islands" },
           new { Latitude = -7.3244, Longitude = 111.5917, FullAddress = "Central Java, Indonesia" },
           new { Latitude = -10.1565, Longitude = 115.2543, FullAddress = "Sumbawa, Indonesia" },
           new { Latitude = -51.100, Longitude = 71.500, FullAddress = "Heard Island, Australia" },
           new { Latitude = 7.5645, Longitude = 50.8989, FullAddress = "Highlands of Papua New Guinea" },
           new { Latitude = -13.4569, Longitude = 48.2787, FullAddress = "Mauritius, Indian Ocean" },
           new { Latitude = 47.6183, Longitude = -56.7449, FullAddress = "Labrador, Canada" },
           new { Latitude = 53.3000, Longitude = -100.5000, FullAddress = "Central Canada" },
           new { Latitude = 45.4825, Longitude = -78.1400, FullAddress = "Northern Ontario, Canada" },
           new { Latitude = 60.056, Longitude = -3.047, FullAddress = "Iceland (remote areas)" },
           new { Latitude = 55.623, Longitude = 25.028, FullAddress = "Turkmenistan Desert, Central Asia" },
           new { Latitude = -7.6495, Longitude = 117.745, FullAddress = "Southeast Sulawesi, Indonesia" },
           new { Latitude = -37.479, Longitude = 107.963, FullAddress = "Atacama Desert, Chile" },
           new { Latitude = 24.374, Longitude = 90.276, FullAddress = "Central Bangladesh, remote area" },
           new { Latitude = 51.908, Longitude = 174.117, FullAddress = "Pacific Ocean, Remote Pacific Island" },
           new { Latitude = 61.145, Longitude = 91.602, FullAddress = "Siberian Taiga, Russia" },
           new { Latitude = 31.750, Longitude = 48.710, FullAddress = "Kerman Province, Iran" },
           new { Latitude = 60.917, Longitude = -28.214, FullAddress = "Iceland, remote location" },
           new { Latitude = 33.125, Longitude = -91.209, FullAddress = "Cypress Creek, Arkansas, USA" },
           new { Latitude = -27.598, Longitude = 116.065, FullAddress = "Western Australia (remote area)" }
        };

        DateTime now = DateTime.Now;

        for (int i = 0; i < 50; i++)
        {
            // בחר מיקום אקראי מתוך המיקומים המוגדרים
            var location = locations[s_rand.Next(0, locations.Length)];

            DateTime openTime = now.AddDays(s_rand.Next(1, 100));  // זמן פתיחה בטווח של 1 עד 100 ימים מעכשיו
            CallType type = (CallType)s_rand.Next(0, 4);  // בחר סוג קריאה אקראי מתוך 4 אפשרויות
            DateTime? closeTime = null;

            // קובעים אם הקריאה תהיה שלא הוקצה, שפג תוקפן או רגילה
            if (unallocatedCalls < 15 && s_rand.NextDouble() < 0.3) // 30% סיכוי לכך שהיא לא תוקצה
            {
                closeTime = null;  // לא הוקצה זמן סיום
                unallocatedCalls++;
            }
            else if (expiredCalls < 5 && s_rand.NextDouble() < 0.1) // 10% סיכוי לכך שהיא תיפג תוקפן
            {
                closeTime = now.AddDays(s_rand.Next(-10, -1));  // זמן סיום בתאריך עבר
                expiredCalls++;
            }
            else
            {
                closeTime = s_rand.NextDouble() < 0.8 ? openTime.AddHours(s_rand.Next(1, 72)) : (DateTime?)null;  // קריאות עם זמן סיום, 80% סיכוי
                allocatedCalls++;
            }

            Call call = new(
             type,                            // callType
             descriptions[(int)type],         // description
             location.FullAddress,            // fullAddress
             location.Latitude,               // latitude
             location.Longitude,              // longitude
             openTime,                        // openTime
             closeTime);                      // maxEndTime (עבר את הזמן הנוכחי)

            s_dal!.Call.Create(call);
        }
    }
    private static void createAssignment()
    {
        var calls = s_dal!.Call.ReadAll();
        var volunteers = s_dal!.Volunteer.ReadAll();
        var callsToAllocate = calls.Skip((int)(calls.Count() * 0.2)).ToList(); // 80% מהקריאות
        var unassignedCalls = calls.Take((int)(calls.Count() * 0.2)).ToList(); // 20% מהקריאות שלא טופלו

        foreach (Call call in callsToAllocate)
        {
            Volunteer randomVolunteer;  // מגרילים מתנדב באופן אקראי
            randomVolunteer = volunteers.ElementAt(s_rand.Next(volunteers.Count()));

            TimeSpan assignmentSpan;
            if (call.MaxEndTime.HasValue)
                assignmentSpan = call.MaxEndTime.Value - call.OpenTime;
            else
                assignmentSpan = TimeSpan.Zero;

            double randomMinutes = s_rand.NextDouble() * assignmentSpan.TotalMinutes;
            DateTime randomStartTime = call.OpenTime.AddMinutes(randomMinutes);
            DateTime? randomEndTime = (s_rand.NextDouble() > 0.5) ? randomStartTime.AddMinutes(s_rand.Next((int)assignmentSpan.TotalMinutes)) : null;

            // אם הזמן הסיום לא קיים, יהיה "ביטול עצמי"
            AssignmentStatus? status;
            if (randomEndTime == null)
                status = AssignmentStatus.SelfCancelled; // שינוי לסטטוס של "ביטול עצמי"
            else if (call.MaxEndTime != null && randomEndTime > call.MaxEndTime)
            {
                status = AssignmentStatus.Completed;
            }
            else
            {
                status = (AssignmentStatus)s_rand.Next(Enum.GetValues(typeof(AssignmentStatus)).Length - 1);
            }

            s_dal!.Assignment.Create(new Assignment(
                call.Id,
                randomVolunteer.Id,
                randomStartTime,
                randomEndTime,
                status
            ));
        }

        // הוספת קריאות שלא טופלו
        foreach (Call call in unassignedCalls)
        {
            s_dal!.Assignment.Create(new Assignment(
            call.Id,
            -1, // אין מתנדב שמטפל בה
            DateTime.MinValue, // לא תהיה תאריך התחלה
            null, // לא יהיה זמן סיום
            null));// לא יהיה סטטוס
        }
    }

}




