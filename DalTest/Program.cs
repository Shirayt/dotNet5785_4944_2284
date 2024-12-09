namespace DalTest;
using Dal;
using DalApi;
using DO;

/// <summary>
/// The test program, to make sure that all classes are defined well.
/// </summary>
internal class Program
{ static readonly IDal s_dal = new DalList(); //stage 2
   
    static void Main(string[] args)
    {
        bool exit = false;
        try
        {
            while (!exit)
            {
                ShowMainMenu();
                int choice = GetUserChoice();
                switch (choice)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        ShowCallMenu();
                        break;
                    case 2:
                        ShowVolunteerMenu();
                        break;
                    case 3:
                        ShowAssignmentMenu();
                        break;
                    case 4:
                        Initialization.Do(s_dal);//stage 2
                        break;
                    case 5:
                        Console.WriteLine("\t\t\t\t\t--------Calls Start--------");
                        ReadAllCalls();
                        Console.WriteLine("\t\t\t\t\t--------Calls End--------");
                        Console.WriteLine("\t\t\t\t\t--------Volunteers Start--------");
                        ReadAllVolunteers();
                        Console.WriteLine("\t\t\t\t\t--------Volunteers End--------");
                        Console.WriteLine("\t\t\t\t\t--------Assignments Start--------");
                        ReadAllAssignments();
                        Console.WriteLine("\t\t\t\t\t--------Assignments End--------");
                        break;
                    case 6:
                        ShowConfigMenu();
                        break;
                    case 7:
                        s_dal.ResetDB();
                        break;
                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
        }
        catch (DalDoesNotExistException ex)
        {
            Console.WriteLine(ex.Message);
        }


    }
    private static void ShowMainMenu()
    {
        Console.WriteLine("Main Menu:");
        Console.WriteLine("0. Exit");
        Console.WriteLine("1. Call Menu");
        Console.WriteLine("2. Volunteer Menu");
        Console.WriteLine("3. Assignment Menu");
        Console.WriteLine("4. Initializing the data");//
        Console.WriteLine("5. Displaying all data in the database");//
        Console.WriteLine("6. Configuration Menu");
        Console.WriteLine("7. Database reset and configuration data reset");//
        Console.Write("Please choose an option: ");
    }
    private static int GetUserChoice()
    {
        int choice;
        while (!int.TryParse(Console.ReadLine(), out choice))
            Console.WriteLine("Invalid input. Please enter a valid number.");
        return choice;
    }
    private static void ShowCallMenu()
    {
        Console.Clear();
        Console.WriteLine("Call Menu:");
        Console.WriteLine("0. Back to Main Menu");
        Console.WriteLine("1. Create Call");
        Console.WriteLine("2. Read Call by ID");
        Console.WriteLine("3. Read All Calls");
        Console.WriteLine("4. Update Call");
        Console.WriteLine("5. Delete Call");
        Console.WriteLine("6. Delete All Calls");
        int choice;
        bool exit = false;
        try
        {
            while (!exit)
            {

                Console.Write("Please choose an option: ");
                choice = GetUserChoice();

                switch (choice)
                {
                    case 0:
                        exit = true;
                        break;
                    case 1:
                        CreateCall();
                        break;
                    case 2:
                        ReadCallById();
                        break;
                    case 3:
                        ReadAllCalls();
                        break;
                    case 4:
                        UpdateCall();
                        break;
                    case 5:
                        DeleteCall();
                        break;
                    case 6:
                        DeleteAllCalls();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
        catch (DalDoesNotExistException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    private static void CreateCall()
    {
        bool inputValid = false;

        while (!inputValid) // הלולאה תמשיך עד שהקלט יהיה תקין
            try
            {
                Console.Write("Enter Call Type (Emergency,Equipment,Doctor,Training): ");
                if (!Enum.TryParse(Console.ReadLine()!, out CallType call_type))
                    throw new FormatException("Call Type is invalid!");
                Console.Write("Enter Call Description: ");
                string description = Console.ReadLine()!;
                Console.Write("Enter Full Address: ");
                string full_address = Console.ReadLine()!;
                Console.Write("Enter Latitude: ");
                if (!double.TryParse(Console.ReadLine(), out double latitude))
                    throw new FormatException("Latitude is invalid!");
                Console.Write("Enter Longitude: ");
                if (!double.TryParse(Console.ReadLine(), out double longitude))
                    throw new FormatException("Longitude is invalid!");
                Console.Write("Enter full open time(dd/mm/yy hh:mm:ss): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime openTime))
                    throw new FormatException("Open time is invalid!");
                Console.Write("Enter full maximum end time(dd/mm/yy hh:mm:ss): ");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endTime))
                    throw new FormatException("End time is invalid!");

                Call newCall = new Call(
                   call_type,
                   description,
                   full_address,
                   latitude,
                   longitude,
                   openTime,
                  endTime);
                s_dal?.Call.Create(newCall);
                Console.WriteLine("Call created successfully.");

                inputValid = true; // יציאה מהלולאה כי הקלט היה תקין
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
    }
    private static void ReadCallById()
    {
        Console.Write("Enter Call ID to read: ");
        int id = int.Parse(Console.ReadLine()!);
        var call = s_dal?.Call.Read(id);
        if (call != null)
        {
            Console.WriteLine($"Call ID: {call.Id}, Call Type: {call.CallType}, " +
                $"Description: {call.Description ?? "N/A"}, Full Address: {call.FullAddress}, " +
                $"Latitude: {call.Latitude}, Longitude: {call.Longitude}, " +
                $"Opening Time: {call.OpenTime}, Max Finish Time: {call.MaxEndTime}");
        }
        else
            Console.WriteLine("Call not found.");
    }
    private static void ReadAllCalls()
    {
        var calls = s_dal?.Call.ReadAll();
        if (calls != null)
            foreach (var call in calls)
            {
                Console.WriteLine($"Call ID: {call.Id}, Call Type: {call.CallType}, " +
                    $"Description: {call.Description ?? "N/A"}, Full Address: {call.FullAddress}, " +
                    $"Latitude: {call.Latitude}, Longitude: {call.Longitude}, " +
                    $"Opening Time: {call.OpenTime}, Max Finish Time: {call.MaxEndTime}");
            }
        else
            Console.WriteLine("No calls found.");
    }
    private static void UpdateCall()
    {
        bool inputValid = false;

        while (!inputValid) // הלולאה תמשיך עד שהקלט יהיה תקין
            try
            {
                Console.Write("Enter Call ID to update: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                    throw new FormatException("Call ID is invalid!");
                var call = s_dal?.Call.Read(id);
                Console.WriteLine(call!);
                if (call != null)
                {
                    Console.Write("Enter Call Type to Update (Emergency,Equipment,Doctor,Training): ");
                    string callTypeInput = Console.ReadLine()!;
                    CallType call_type = string.IsNullOrEmpty(callTypeInput) || !Enum.TryParse(callTypeInput, out CallType cType) ? call.CallType : cType;
                    Console.Write("Enter Call Description to Update: ");
                    string descriptionInput = Console.ReadLine()!;
                    string description = string.IsNullOrEmpty(descriptionInput) ? call.Description : descriptionInput;
                    Console.Write("Enter Full Address To update: ");
                    string fullAddressInput = Console.ReadLine()!;
                    string fullAddress = string.IsNullOrEmpty(fullAddressInput) ? call.FullAddress : fullAddressInput;
                    Console.WriteLine("Enter Latitude to update: ");
                    string LatitudeInput = Console.ReadLine()!;
                    double latitude = string.IsNullOrEmpty(LatitudeInput) || !double.TryParse(LatitudeInput, out double latit) ? call.Latitude : latit;
                    Console.WriteLine("Enter Longitude to update: ");
                    string LongitudeInput = Console.ReadLine()!;
                    double longitude = string.IsNullOrEmpty(LongitudeInput) || !double.TryParse(LongitudeInput, out double longit) ? call.Longitude : longit;
                    Console.Write("Enter full open time(dd/MM/yy HH:mm:ss): ");
                    string openTimeInput = Console.ReadLine()!;
                    DateTime openTime = DateTime.ParseExact(openTimeInput, "dd/MM/yy HH:mm:ss", null);
                    Console.Write("Enter full maximum end time(dd/MM/yy HH:mm:ss): ");
                    string endTimeInput = Console.ReadLine()!;
                    DateTime endTime = DateTime.ParseExact(endTimeInput, "dd/MM/yy HH:mm:ss", null);



                    Call newCall = new Call(
                      call_type,
                      description,
                      fullAddress,
                      latitude,
                      longitude,
                      openTime,
                      endTime
                      );

                    s_dal?.Call.Update(newCall);
                    Console.WriteLine("Call updated successfully.");
                    Console.WriteLine(newCall);
                }
                else
                    Console.WriteLine("Call not found.");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
    }
    private static void DeleteCall()
    {
        Console.Write("Enter Call ID to delete: ");
        int id = int.Parse(Console.ReadLine()!);
        s_dal?.Call.Delete(id);
        Console.WriteLine("Call deleted successfully.");
    }
    private static void DeleteAllCalls()
    {
        s_dal?.Call.DeleteAll();
        Console.WriteLine("All calls deleted successfully.");
    }
    private static void ShowVolunteerMenu()
    {
        Console.Clear();
        Console.WriteLine("Volunteer Menu:");
        Console.WriteLine("0. Back to Main Menu");
        Console.WriteLine("1. Create Volunteer");
        Console.WriteLine("2. Read Volunteer by ID");
        Console.WriteLine("3. Read All Volunteers");
        Console.WriteLine("4. Update Volunteer");
        Console.WriteLine("5. Delete Volunteer");
        Console.WriteLine("6. Delete All Volunteers");
        bool exit = false;
        int choice;
        while (!exit)
        {
            Console.Write("Please choose an option: ");
            choice = GetUserChoice();
            switch (choice)
            {
                case 0:
                    exit = true;
                    break;
                case 1:
                    CreateVolunteer();
                    break;
                case 2:
                    ReadVolunteerById();
                    break;
                case 3:
                    ReadAllVolunteers();
                    break;
                case 4:
                    UpdateVolunteer();
                    break;
                case 5:
                    DeleteVolunteer();
                    break;
                case 6:
                    DeleteAllVolunteers();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
    private static void CreateVolunteer()
    {
        bool inputValid = false;

        while (!inputValid) // הלולאה תמשיך עד שהקלט יהיה תקין
            try
            {
                Console.Write("Enter Volunteer ID: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                    throw new FormatException("ID is invalid!");
                Console.Write("Enter phone Number: ");
                string phoneNumber = Console.ReadLine()!;
                if (string.IsNullOrEmpty(phoneNumber) || phoneNumber.Length < 10)
                    throw new FormatException("phone number is invalid!");
                Console.Write("Enter Full Name: ");
                string fullName = Console.ReadLine()!;
                if (string.IsNullOrEmpty(fullName))
                    throw new FormatException("Full Name is required!");
                Console.Write("Enter Email: ");
                string email = Console.ReadLine()!;
                Console.Write("Enter Full Address: ");
                string? fullAddress = Console.ReadLine();
                Console.Write("Enter Latitude: ");
                if (!double.TryParse(Console.ReadLine(), out double latitude))
                    throw new FormatException("Latitude is invalid!");
                Console.Write("Enter Longitude: ");
                if (!double.TryParse(Console.ReadLine(), out double longitude))
                    throw new FormatException("Longitude is invalid!");
                Console.Write("Enter Role (Manager/Volunteer): ");
                if (!Enum.TryParse(Console.ReadLine(), out Role role))
                    throw new FormatException("Role is invalid!");
                Console.Write("Enter if Is Active (true/false): ");
                if (!bool.TryParse(Console.ReadLine(), out bool isActive))
                    throw new FormatException("IsActive is invalid!");
                Console.Write("Enter Distance Type (Air,Walk,Drive): ");
                if (!Enum.TryParse(Console.ReadLine()!, true, out DistanceType distanceType))
                    throw new FormatException("DistanceType is invalid!");
                Console.Write("Enter Max Distance (km): ");
                double? maxDistance = null;
                string maxDistanceInput = Console.ReadLine()!;
                if (!double.TryParse(maxDistanceInput, out double parsedMaxDistance))
                    throw new FormatException("Max Distance is invalid!");
                maxDistance = parsedMaxDistance;
                Console.Write("Enter Password (optional): ");
                string? password = Console.ReadLine();
                Volunteer newVolunteer;

                if (password == "")
                {
                    newVolunteer = new Volunteer(
                    id,
                    fullName,
                    phoneNumber,
                    email,
                    fullAddress,
                    latitude,
                    longitude,
                    role,
                    isActive,
                    maxDistance,
                    distanceType);
                }

                else
                {
                    newVolunteer = new Volunteer(
                   id,
                   fullName,
                   phoneNumber,
                   email,
                   fullAddress,
                   latitude,
                   longitude,
                   role,
                   isActive,
                   maxDistance,
                   distanceType,
                   password);
                };
                s_dal?.Volunteer.Create(newVolunteer);
                Console.WriteLine("Volunteer created successfully.");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

    }
    private static void ReadVolunteerById()
    {
        Console.Write("Enter Volunteer ID to read: ");
        int id = int.Parse(Console.ReadLine()!);
        var volunteer = s_dal?.Volunteer.Read(id);
        if (volunteer != null)
        {
            Console.WriteLine($"Volunteer ID: {volunteer.Id}, Full Name: {volunteer.FullName}, phone: {volunteer.PhoneNumber}, Email: {volunteer.Email}, " +
                $"Full Address: {volunteer.CurrentFullAddress}, Latitude: {volunteer.Latitude}, Longitude: {volunteer.Longitude}, " +
                $"Role: {volunteer.Role}, Active: {volunteer.IsActive}, Distance Type: {volunteer.DistanceType}, " +
                $"Max Distance: {volunteer.MaxDistanceForCall}, Password: {volunteer.Password}");
        }
        else
            Console.WriteLine("Volunteer not found.");
    }
    private static void ReadAllVolunteers()
    {
        var volunteers = s_dal?.Volunteer.ReadAll();
        if (volunteers != null)
        {
            int i = 1;
            foreach (var volunteer in volunteers)
            {
                Console.WriteLine($"volunteer{i++}");
                Console.WriteLine($"Volunteer ID: {volunteer.Id}, Full Name: {volunteer.FullName}, phone: {volunteer.PhoneNumber}, Email: {volunteer.Email}, " +
               $"Full Address: {volunteer.CurrentFullAddress}, Latitude: {volunteer.Latitude}, Longitude: {volunteer.Longitude}, " +
               $"Role: {volunteer.Role}, Active: {volunteer.IsActive}, Distance Type: {volunteer.DistanceType}, " +
               $"Max Distance: {volunteer.MaxDistanceForCall}, Password: {volunteer.Password}");
            }
        }
        else
            Console.WriteLine("No volunteers found.");
    }
    private static void UpdateVolunteer()
    {
        bool inputValid = false;

        while (!inputValid) // הלולאה תמשיך עד שהקלט יהיה תקין
            try
            {
                Console.Write("Enter Volunteer ID to update: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                    throw new FormatException("Volunteer ID is invalid!");
                var volunteer = s_dal?.Volunteer.Read(id);
                Console.WriteLine(volunteer);
                if (volunteer != null)
                {
                    Console.WriteLine("Enter Volunteer Full name to update: ");
                    string fullName = Console.ReadLine()!;
                    Console.WriteLine("Enter Distance Type(Air,Walk,Drive) to update: ");
                    string distanceTypeInput = Console.ReadLine()!;
                    DistanceType distanceTypes = string.IsNullOrEmpty(distanceTypeInput) || !Enum.TryParse(distanceTypeInput, out DistanceType dType) ? volunteer.DistanceType : dType;
                    Console.WriteLine("Enter Volunteer Role(Manager/Volunteer) to update: ");
                    string roleInput = Console.ReadLine()!;
                    Role role = string.IsNullOrEmpty(roleInput) || !Enum.TryParse(roleInput, out Role rType) ? volunteer.Role : rType;
                    Console.WriteLine("Enter phone number to update: ");
                    string phoneNumber = Console.ReadLine()!;
                    Console.WriteLine("Enter Volunteer Email to update: ");
                    string email = Console.ReadLine()!;
                    Console.WriteLine("Enter Volunteer FullAddress to update: ");
                    string fullAddress = Console.ReadLine()!;
                    Console.WriteLine("Enter Volunteer Password to update: ");
                    string password = Console.ReadLine()!;
                    Console.WriteLine("Enter Latitude to update: ");
                    string LatitudeInput = Console.ReadLine()!;
                    double? latitude = string.IsNullOrEmpty(LatitudeInput) || !double.TryParse(LatitudeInput, out double lat) ? volunteer.Latitude : lat;
                    Console.WriteLine("Enter Longitude to update: ");
                    string longitudeInput = Console.ReadLine()!;
                    double? longitude = string.IsNullOrEmpty(longitudeInput) || !double.TryParse(longitudeInput, out double longit) ? volunteer.Longitude : longit; ;
                    Console.WriteLine("Enter Max distance to update: ");
                    string maxDistanceInput = Console.ReadLine()!;
                    double? maxDistance = string.IsNullOrEmpty(maxDistanceInput) || !double.TryParse(maxDistanceInput, out double maxDis) ? volunteer.MaxDistanceForCall : maxDis;
                    Console.WriteLine("Enter Active to update: ");
                    string isActiveInput = Console.ReadLine()!;
                    bool isActive = string.IsNullOrEmpty(isActiveInput) || !bool.TryParse(isActiveInput, out bool isA) ? volunteer.IsActive : isA;
                    Volunteer newVolunteer = new Volunteer(
                         volunteer.Id,
                         string.IsNullOrEmpty(fullName) ? volunteer.FullName : fullName,
                         string.IsNullOrEmpty(phoneNumber) ? volunteer.PhoneNumber : phoneNumber,
                         string.IsNullOrEmpty(email) ? volunteer.Email : email,
                         string.IsNullOrEmpty(fullAddress) ? volunteer.CurrentFullAddress : fullAddress,
                         latitude,
                         longitude,
                         (role != Role.Manager) && (role != Role.Volunteer) ? volunteer.Role : role,
                         isActive ? volunteer.IsActive : isActive,
                         maxDistance,
                         (distanceTypes != DistanceType.Drive) && (distanceTypes != DistanceType.Drive) && (distanceTypes != DistanceType.Walk) ? volunteer.DistanceType : distanceTypes,
                         string.IsNullOrEmpty(password) ? volunteer.Password : password
                    );
                    s_dal?.Volunteer.Update(newVolunteer);
                    Console.WriteLine("Volunteer updated successfully.");
                    Console.WriteLine(newVolunteer);
                }
                else
                    Console.WriteLine("Volunteer not found.");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
    }
    private static void DeleteVolunteer()
    {
        Console.Write("Enter Volunteer ID to delete: ");
        int id = int.Parse(Console.ReadLine()!);
        s_dal?.Volunteer.Delete(id);
        Console.WriteLine("Volunteer deleted successfully.");
    }
    private static void DeleteAllVolunteers()
    {
        s_dal?.Volunteer.DeleteAll();
        Console.WriteLine("All volunteers deleted successfully.");
    }
    private static void ShowAssignmentMenu()
    {
        Console.Clear();
        Console.WriteLine("Assignment Menu:");
        Console.WriteLine("0. Back to Main Menu");
        Console.WriteLine("1. Create Assignment");
        Console.WriteLine("2. Read Assignment by ID");
        Console.WriteLine("3. Read All Assignments");
        Console.WriteLine("4. Update Assignment");
        Console.WriteLine("5. Delete Assignment");
        Console.WriteLine("6. Delete All Assignments");
        bool exit = false;
        int choice;
        while (!exit)
        {
            Console.Write("Please choose an option: ");
            choice = GetUserChoice();
            switch (choice)
            {
                case 0:
                    exit = true;
                    break;
                case 1:
                    CreateAssignment();
                    break;
                case 2:
                    ReadAssignmentById();
                    break;
                case 3:
                    ReadAllAssignments();
                    break;
                case 4:
                    UpdateAssignment();
                    break;
                case 5:
                    DeleteAssignment();
                    break;
                case 6:
                    DeleteAllAssignments();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
    private static void CreateAssignment()
    {
        bool inputValid = false;

        while (!inputValid) // הלולאה תמשיך עד שהקלט יהיה תקין
            try
            {
                Console.WriteLine("Enter Volunteer ID: ");
                if (!int.TryParse(Console.ReadLine()!, out int volunteerId) || (s_dal.Volunteer.Read(volunteerId) == null))
                    throw new FormatException("Volunteer ID is invalid!");
                Console.WriteLine("Enter Call ID: ");
                if (!int.TryParse(Console.ReadLine()!, out int callId) || (s_dal.Call.Read(callId) == null))
                    throw new FormatException("Call ID is invalid!");
                Console.Write("Enter start time (dd/MM/yyyy HH:mm:ss): ");
                string startTimeInput = Console.ReadLine()!;
                if (!DateTime.TryParse(startTimeInput, out DateTime startTime) || (s_dal.Call.Read(callId) == null))
                    throw new FormatException("start time is invalid!");

                Assignment newAssignment = new Assignment(callId, volunteerId, startTime);
                s_dal!.Assignment.Create(newAssignment);
                Console.WriteLine("An Assignment was successfully created.");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
    }
    private static void ReadAssignmentById()
    {
        Console.Write("Enter Assignment ID to read: ");
        int id = int.Parse(Console.ReadLine()!);
        var assignment = s_dal?.Assignment.Read(id);
        if (assignment != null)
        {
            Console.WriteLine($"Assignment ID: {assignment.Id}, Volunteer ID: {assignment.VolunteerId}, " +
                $"Call Id: {assignment.CallId},  Start Time: {assignment.StartTime}, " +
                $"End Time: {assignment.EndTime}, End Type: {assignment.Status}, ");
        }
        else
            Console.WriteLine("Assignment not found.");
    }
    private static void ReadAllAssignments()
    {
        var assignments = s_dal?.Assignment.ReadAll();
        if (assignments != null)
        {
            foreach (var assignment in assignments)
            {
                Console.WriteLine($"Assignment ID: {assignment.Id}, Volunteer ID: {assignment.VolunteerId}, " +
                    $"Call Id: {assignment.CallId},  Start Time: {assignment.StartTime}, " +
                    $"End Time: {assignment.EndTime}, End Type: {assignment.Status}, ");
            }
        }
        else
            Console.WriteLine("No assignments found.");
    }
    private static void UpdateAssignment()
    {
        bool inputValid = false;

        while (!inputValid) // הלולאה תמשיך עד שהקלט יהיה תקין
            try
            {
                Console.Write("Enter Assignment ID to update: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                    throw new FormatException("Call ID is invalid!");
                var assignment = s_dal?.Assignment.Read(id);
                if (assignment != null)
                {
                    Console.WriteLine(assignment);
                    Console.Write("Enter volunteer id:  ");
                    string volunteerIdInput = Console.ReadLine()!;
                    int volunteerId = string.IsNullOrEmpty(volunteerIdInput) || !int.TryParse(volunteerIdInput, out int vId) ? assignment.VolunteerId : vId;
                    Console.Write("Enter call id:  ");
                    string callIdInput = Console.ReadLine()!;
                    int callId = string.IsNullOrEmpty(callIdInput) || !int.TryParse(callIdInput, out int cId) ? assignment.CallId : cId;
                    Console.Write("Enter Start time:(dd/mm/yy hh:mm:ss) ");
                    string startTimeInput = Console.ReadLine()!;
                    DateTime? startTime = string.IsNullOrEmpty(startTimeInput) || !DateTime.TryParse(startTimeInput, out DateTime ent) ? assignment.StartTime : ent;
                    Console.Write("Enter end time (dd/mm/yy hh:mm:ss): ");
                    string endTimeInput = Console.ReadLine()!;
                    DateTime? endTime = string.IsNullOrEmpty(endTimeInput) || !DateTime.TryParse(endTimeInput, out DateTime end) ? assignment.EndTime : end;
                    Console.Write("Enter end type( Completed,SelfCancelled, ManagerCancelled,Expired ): ");
                    string endTypeInput = Console.ReadLine()!;
                    AssignmentStatus? endType = string.IsNullOrEmpty(endTypeInput) || !Enum.TryParse(endTypeInput, out AssignmentStatus eType) ? assignment.Status : eType;
                    Assignment newAssignment = new Assignment(
                     callId,
                     volunteerId,
                     startTime ?? assignment.StartTime,
                     endTime ?? assignment.EndTime,
                     (endType != AssignmentStatus.Expired) && (endType != AssignmentStatus.ManagerCancelled) && (endType != AssignmentStatus.SelfCancelled) && (endType != AssignmentStatus.Completed) ? assignment.Status : endType
        );
                    s_dal?.Assignment.Update(newAssignment);
                    Console.WriteLine("Assignment updated successfully.");
                    Console.WriteLine(newAssignment);
                }
                else
                    Console.WriteLine("Assignment not found.");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
    }
    private static void DeleteAssignment()
    {
        Console.Write("Enter Assignment ID to delete: ");
        int id = int.Parse(Console.ReadLine()!);
        s_dal?.Assignment.Delete(id);
        Console.WriteLine("Call deleted successfully.");
    }
    private static void DeleteAllAssignments()
    {
        s_dal?.Assignment.DeleteAll();
        Console.WriteLine("All are assignments successfully deleted.");
    }
    private static void ShowConfigMenu()
    {
        Console.Clear();
        Console.WriteLine("Configuration Menu:");
        Console.WriteLine("0. Back to Main Menu");
        Console.WriteLine("1. Advance system clock by one minute");
        Console.WriteLine("2. Advance system clock by one hour");
        Console.WriteLine("3. Advance system clock by one month");
        Console.WriteLine("4. Advance system clock by one year");
        Console.WriteLine("5. Show current system clock value");
        Console.WriteLine("6. Set a new value for any configuration variable");
        Console.WriteLine("7. Show current value for any configuration variable");
        Console.WriteLine("8. Reset values ​​for all configuration variables");
        bool exit = false;
        int choice;
        while (!exit)
        {
            Console.Write("Please choose an option: ");
            choice = GetUserChoice();
            switch (choice)
            {
                case 0:
                    exit = true;
                    break;
                case 1:
                    s_dal!.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                    break;
                case 2:
                    s_dal!.Config.Clock = s_dal.Config.Clock.AddHours(1);
                    break;
                case 3:
                    s_dal!.Config.Clock = s_dal.Config.Clock.AddMonths(1);
                    break;
                case 4:
                    s_dal!.Config.Clock = s_dal.Config.Clock.AddYears(1);
                    break;
                case 5:
                    Console.WriteLine(s_dal!.Config.Clock);
                    break;
                case 6:
                    SetClockOrRiskRange();
                    break;
                case 7:
                    ShowClockOrRiskRange();
                    break;
                case 8:
                    ResetConfig();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }
    private static void SetClockOrRiskRange()
    {
        bool inputValid = false;

        while (!inputValid) // הלולאה תמשיך עד שהקלט יהיה תקין
            try
            {
                Console.WriteLine("Enter C/R for to choose Clock or RiskRange: ");
                if (!char.TryParse(Console.ReadLine(), out char choice))
                    throw new FormatException("Invalid input. Please enter a single character.");
                switch (choice)
                {
                    case 'C':
                        Console.WriteLine("Enter new Date: ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime newClock))
                        {
                            throw new FormatException("Invalid input. Please enter a single character.");
                        }
                        s_dal!.Config.Clock = newClock;
                        break;
                    case 'R':
                        Console.WriteLine("Enter new Time Span: ");
                        if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan newRiskRange))
                        {
                            throw new FormatException("Invalid input. Please enter a single character.");
                        }
                        s_dal!.Config.RiskRange = newRiskRange;
                        break;
                    default:
                        Console.WriteLine("Wrong choice!");
                        break;
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
    }
    private static void ShowClockOrRiskRange()
    {
        bool inputValid = false;

        while (!inputValid) // הלולאה תמשיך עד שהקלט יהיה תקין
            try
            {
                Console.WriteLine("Enter C/R for to choose Clock or RiskRange: ");
                if (!char.TryParse(Console.ReadLine(), out char choice))
                    throw new FormatException("Invalid input. Please enter a single character.");
                switch (choice)
                {
                    case 'C':
                        Console.WriteLine(s_dal!.Config.Clock);
                        break;
                    case 'R':
                        Console.WriteLine(s_dal!.Config.RiskRange);
                        break;
                    default:
                        Console.WriteLine("Wrong choice!");
                        break;
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
    }
    private static void ResetConfig()
    {
        s_dal?.Config.Reset();
        Console.WriteLine("Configuration has been reset.");
    }


}
