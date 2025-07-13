namespace DalTest;
using DalApi;
using DO;
using System.Linq;

/// <summary>
/// Initialize the data system by creating objects
/// </summary>
public static class Initialization
{
    private static IDal? s_dal;
    private static readonly Random s_rand = new();

    //public static void Do(IDal dal) //stage 2
    public static void Do() //stage 4
    {
        try
        {
            //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); // stage 2
            s_dal = DalApi.Factory.Get; //stage 4
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

        string[] volunteerNames = { "Dani Levy", "Shira Cohen", "Tali Amar", "Yair Israelof", "Eli Klein", "Maya Shimon", "Avi Ben-David", "Liat Ron", "Yaara Berman", "Noa Sasson", "Roi Peretz", "Gal Zohar", "Dana Kfir", "Yosef Shaked", "Ruth Erez", "Or Ben-Haim", "Noga Azulai", "Itay Levi", "Yasmin David", "Tomer Avital", "Lina Khalil", "Bar Refaeli", "Nadav Natan", "Sivan Tal", "Ron Arad", "Naomi Feldman", "Erez Dahan", "Tamar Peleg", "Yoni Azulai", "Lior Hadad", "Shani Michaeli", "Eitan Nave", "Roni Segal", "Tal Shahar", "Niv Avrahami", "Adi Mor", "Kfir Lavi", "Yael Shlush", "Itamar Dayan", "Meital Armon", "Gil Rave", "Talia Mizrahi", "Omri Shaul", "Vered Sharabi", "Alon Amram" };
        string[] emails = { "dani@gmail.com", "shira@gmail.com", "tali@gmail.com", "yair@gmail.com", "eli@gmail.com", "maya@gmail.com", "avi@gmail.com", "liat@gmail.com", "yaara@gmail.com", "noa@gmail.com", "roi@gmail.com", "gal@gmail.com", "dana@gmail.com", "yosef@gmail.com", "ruth@gmail.com", "or@gmail.com", "noga@gmail.com", "itay@gmail.com", "yasmin@gmail.com", "tomer@gmail.com", "lina@gmail.com", "bar@gmail.com", "nadav@gmail.com", "sivan@gmail.com", "ron@gmail.com", "naomi@gmail.com", "erez@gmail.com", "tamar@gmail.com", "yoni@gmail.com", "lior@gmail.com", "shani@gmail.com", "eitan@gmail.com", "roni@gmail.com", "tal@gmail.com", "niv@gmail.com", "adi@gmail.com", "kfir@gmail.com", "yael@gmail.com", "itamar@gmail.com", "meital@gmail.com", "gil@gmail.com", "talia@gmail.com", "omri@gmail.com", "vered@gmail.com", "alon@gmail.com" };
        string[] phones = { "0541111111", "0542222222", "0543333333", "0544444444", "0545555555", "0546666666", "0547777777", "0548888888", "0549999999", "0550000000", "0551111111", "0552222222", "0553333333", "0554444444", "0555555555", "0556666666", "0557777777", "0558888888", "0559999999", "0560000000", "0561111111", "0562222222", "0563333333", "0564444444", "0565555555", "0566666666", "0567777777", "0568888888", "0569999999", "0570000000", "0571111111", "0572222222", "0573333333", "0574444444", "0575555555", "0576666666", "0577777777", "0578888888", "0579999999", "0580000000", "0581111111", "0582222222", "0583333333", "0584444444", "0585555555" };
        string[] addresses = { "29 Chinggis Avenue, Ulaanbaatar, Mongolia", "2 Tindall Street, Nelson, New Zealand", "Via Lago di Bracciano 32, 00069 Trevignano Romano, Italy", "145 Kulusuk, Greenland", "Vorshikhinskaya Street 11, Yakutsk, Russia", "Calle de los Reyes Católicos, 23, Fuerteventura, Spain", "9 Ngong Road, Nairobi, Kenya", "Bramber, West Sussex, England, United Kingdom", "Station Road, Tindivanam, Tamil Nadu, India", "Boko Haram Village, Borno State, Nigeria", "Lima Norte, Lima, Peru", "Pico de Orizaba, Veracruz, Mexico", "Desolation Sound, British Columbia, Canada", "Toraja, Sulawesi, Indonesia", "Mount Roraima, Venezuela, Brazil, Guyana", "HaPalmach 3, Haifa, Israel", "Bnei Dan 42, Tel Aviv, Israel", "Herzl 17, Netanya, Israel", "Harav Kook 8, Be'er Sheva, Israel", "Derech Eretz 21, Eilat, Israel", "Kedem 12, Lod, Israel", "Yefe Nof 9, Holon, Israel", "Abarbanel 15, Acre, Israel", "Hashalom 10, Kiryat Shmona, Israel", "Ein Gedi 5, Dimona, Israel", "Ben Gurion 10, Ashdod, Israel", "Hertzl 22, Petah Tikva, Israel", "Begin 5, Bat Yam, Israel", "Yavne 3, Ramat Gan, Israel", "Arlozorov 1, Rehovot, Israel", "Nahal Sorek 15, Modi'in, Israel", "Haganah 4, Ashkelon, Israel", "Rothschild 8, Beit Shemesh, Israel", "Yitzhak Rabin 20, Karmiel, Israel", "Harakevet 6, Nahariya, Israel", "Yehuda Halevi 7, Netivot, Israel", "Bar Kochva 9, Afula, Israel", "Rashi 16, Hod Hasharon, Israel", "Yigal Alon 3, Kiryat Gat, Israel", "Tamar 12, Tiberias, Israel", "Ahad Ha'am 2, Sderot, Israel", "Hazayit 13, Arad, Israel", "Habonim 14, Beit She'an, Israel", "Sokolov 5, Ma'alot, Israel", "Gan Ha’ir 11, Zikhron Ya'akov, Israel" };
        double[] latitudes = { 47.8862, -40.5170, 42.1350, 65.6233, 62.0360, 28.4320, -1.2864, 50.8610, 12.2069, 12.1776, -12.0450, 19.0299, 57.3000, 2.4769, 6.7350, 32.7940, 32.0961, 32.3215, 31.2520, 29.5581, 31.9511, 32.0158, 32.9236, 33.2081, 31.0695, 31.8014, 32.0883, 32.0163, 32.0838, 31.8948, 31.9000, 31.6688, 31.7394, 32.9256, 33.0054, 31.4200, 32.6084, 32.1463, 31.6100, 32.7922, 31.5235, 31.2596, 32.4990, 33.0161, 32.5672 };
        double[] longitudes = { 106.9057, 173.2885, 12.2715, -37.4594, 129.7327, -13.8597, 36.8250, -0.3865, -0.2976, 13.4918, -77.2749, -97.2071, -60.8103, -60.6933, -60.7371, 34.9896, 34.7980, 34.8532, 34.7913, 34.9519, 34.8881, 34.7722, 35.0717, 35.5681, 35.0326, 34.6435, 34.8864, 34.7451, 34.8205, 34.8080, 35.0104, 34.5745, 34.9941, 35.3066, 35.0945, 34.5881, 35.2905, 34.8891, 34.7680, 35.5281, 34.5910, 35.2137, 35.5001, 35.2640, 34.9565 };
        try
        {
            for (int i = 0; i < volunteerNames.Length; i++)
            {
                int id;

                do
                {
                    id = s_rand.Next(200000000, 400000000);
                } while (s_dal!.Volunteer.Read(id) != null || id.ToString().PadLeft(9, '0').Reverse().Select((c, i) =>
                {
                    int n = (c - '0') * ((i % 2) + 1);
                    return n > 9 ? n - 9 : n;
                }).Sum() % 10 != 0);

                // Randomly generate the maximum distance for receiving a signal in the range of 5 to 100
                double maxDistanceForCall = s_rand.Next(5, 101);

                // create volunteer object
                Volunteer volunteer = new Volunteer(
                    id,                                // Id
                    volunteerNames[i],                 // FullName
                    phones[i],                         // PhoneNumber
                    emails[i],                         // Email
                    addresses[i],                      // CurrentFullAddress
                    latitudes[i],                      // Latitude
                    longitudes[i],                     // Longitude
                    Role.Volunteer,                    // Role
                    true,                              // IsActive
                    maxDistanceForCall,                // MaxDistanceForCall
                    DistanceType.Air                   // DistanceType
                );
                // Adding the volunteer to the system
                s_dal!.Volunteer.Create(volunteer);
            }
        }
        catch (DalAlreadyExistsException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }


        // Creating a special volunteer of type Manager
        Volunteer ManagerVolunteer = new Volunteer(
            215042284,                             // Id
            "Shira Taviv!",                        // FullName
            "0534164314",                          // PhoneNumber
            "shirayt100@gmail.com",                // Email
            "Pinchas Kehati 12, Jerusalem, Israel",// CurrentFullAddress
            31.7995,                               // Latitude
            35.2115,                               // Longitude
            Role.Manager,                          // Role
            true,                                  // IsActive
            68,                                    // MaxDistanceForCall
            DistanceType.Air,                      // DistanceType
            "Shira100"                             //Password
        );

        s_dal!.Volunteer.Create(ManagerVolunteer);
    }
    private static void createCall()
    {
        // Generate at least 50 calls
        int allocatedCalls = 0;     // Counts calls that were allocated
        int unallocatedCalls = 0;   // Counts calls that were not allocated
        int expiredCalls = 0;       // Counts calls that expired

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

        DateTime now = s_dal!.Config.Clock;

        for (int i = 0; i < 50; i++)
        {
            // Select a random location from the predefined locations
            var location = locations[s_rand.Next(0, locations.Length)];

            DateTime openTime = now.AddDays(-s_rand.Next(1, 50)); // Open time between 1 and 50 days ago
            CallType type = (CallType)s_rand.Next(0, 4);          // Randomly select a call type from 4 options
            DateTime? closeTime = null;

            // Determine if the call will be unallocated, expired, or regular
            if (unallocatedCalls < 15 && s_rand.NextDouble() < 0.4) // 40% chance for unallocated, up to 15
            {
                closeTime = null;  // No close time set (unallocated)
                unallocatedCalls++;
            }
            else if (expiredCalls < 5 && s_rand.NextDouble() < 0.1) // 10% chance for expired, up to 5
            {
                closeTime = now.AddDays(s_rand.Next(-10, -1));  // Close time in the past
                expiredCalls++;
            }
            else
            {
                // 80% chance to have a close time between 60 and 100 days after open time
                closeTime = s_rand.NextDouble() < 0.8 ? openTime.AddDays(s_rand.Next(60, 100)) : (DateTime?)null;
                allocatedCalls++;
            }

            Call call = new(
             type,                            // callType
             descriptions[(int)type],         // description
             location.FullAddress,            // fullAddress
             location.Latitude,               // latitude
             location.Longitude,              // longitude
             openTime,                        // openTime
             closeTime);                      // maxEndTime 

            s_dal!.Call.Create(call);
        }
    }
    private static void createAssignment()
    {
        var calls = s_dal!.Call.ReadAll();
        var volunteers = s_dal!.Volunteer.ReadAll();

        var assignedVolunteerIds = new HashSet<int>();
        var callsToAllocate = calls.Skip((int)(calls.Count() * 0.2)).ToList(); // 80% of the calls
        var unassignedCalls = calls.Take((int)(calls.Count() * 0.2)).ToList(); // 20% of the calls left unassigned

        foreach (Call call in callsToAllocate)
        {
            var availableVolunteers = volunteers
                .Where(v => !assignedVolunteerIds.Contains(v.Id))
                .ToList();

            if (!availableVolunteers.Any())
                break;

            Volunteer randomVolunteer = availableVolunteers[s_rand.Next(availableVolunteers.Count)];
            assignedVolunteerIds.Add(randomVolunteer.Id);

            TimeSpan assignmentSpan;
            if (call.MaxEndTime.HasValue)
                assignmentSpan = call.MaxEndTime.Value - call.OpenTime;
            else
                assignmentSpan = TimeSpan.Zero;

            double totalMinutes = Math.Max(1, assignmentSpan.TotalMinutes); // Ensure at least 1 minute
            DateTime randomStartTime = call.OpenTime.AddMinutes(s_rand.Next((int)totalMinutes));
            double totalDays = Math.Max(1, assignmentSpan.TotalDays);
            int endOffsetDays = s_rand.Next(1, (int)totalDays);
            DateTime? randomEndTime = (s_rand.NextDouble() > 0.5)
? (call.MaxEndTime.HasValue && randomStartTime.AddDays(endOffsetDays) > call.MaxEndTime.Value
       ? call.MaxEndTime
       : randomStartTime.AddDays(endOffsetDays))
: null;


            DateTime now = s_dal!.Config.Clock;


            AssignmentStatus? status;
            if (randomEndTime != null)
            {
                if (randomEndTime == null && now > call.MaxEndTime) //if now > call.MaxEndTime ->Expired
                {
                    status = AssignmentStatus.Expired;
                }
                else
                {
                    var statuses = new[] { AssignmentStatus.SelfCancelled,
                                           AssignmentStatus.Completed,
                                           AssignmentStatus.ManagerCancelled };

                    int index = s_rand.Next(statuses.Length);
                    status = statuses[index];
                }
            }
            else
            {
                status = null; // empty property for now 
            }

            s_dal!.Assignment.Create(new Assignment(
           call.Id,
           randomVolunteer.Id,
           randomStartTime,
           randomEndTime,
           status
       ));
        }

        foreach (Call call in unassignedCalls)
        {
            var availableVolunteers = volunteers
                .Where(v => !assignedVolunteerIds.Contains(v.Id))
                .ToList();

            if (!availableVolunteers.Any())
                break;

            Volunteer randomVolunteer = availableVolunteers[s_rand.Next(availableVolunteers.Count)];
            assignedVolunteerIds.Add(randomVolunteer.Id);

            s_dal!.Assignment.Create(new Assignment(
                call.Id,
                randomVolunteer.Id,
                DateTime.MinValue, // No start time
                null,              // No end time
                null));            // No completion status 
        }

    }
}




