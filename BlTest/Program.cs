using System;
using System.Numerics;
using System.Xml.Linq;
using BlApi;
namespace BlTest
{
    class Program
    {
        static readonly IBl s_bl = Factory.Get();
        static void Main()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("\n--- BL Test System ---");
                    Console.WriteLine("1. Administration");
                    Console.WriteLine("2. Volunteers");
                    Console.WriteLine("3. Calls");
                    Console.WriteLine("0. Exit");
                    Console.Write("Choose an option: ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                AdminMenu();
                                break;
                            case 2:
                                VolunteerMenu();
                                break;
                            case 3:
                                CallMenu();
                                break;
                            case 0:
                                return;
                            default:
                                Console.WriteLine("Invalid choice. Try again.");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while displaying the menu: " + ex.Message);
            }
        }


        static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Administration ---");
                Console.WriteLine("1. Reset Database");
                Console.WriteLine("2. Initialize Database");
                Console.WriteLine("3. Forward Clock");
                Console.WriteLine("4. Get Clock");
                Console.WriteLine("5. Get Risk Time Range");
                Console.WriteLine("6. Set Risk Time Range");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                try
                {
                    switch (choice)
                    {
                        case 1:
                            s_bl.Admin.ResetDB();
                            Console.WriteLine("Database reset successfully");
                            break;
                        case 2:
                            s_bl.Admin.InitializeDB();
                            Console.WriteLine("Database initialized successfully");
                            break;
                        case 3:
                            Console.Write("Enter time unit (Minute, Hour, Day, Month, Year): ");
                            if (Enum.TryParse(Console.ReadLine(), true, out BO.TimeUnit timeUnit))
                            {
                                s_bl.Admin.ForwardClock(timeUnit);
                                Console.WriteLine("System clock advanced.");
                            }
                            else
                            {
                                throw new FormatException("Invalid time unit. Please enter: Minute, Hour, Day, Month, Year.");
                            }
                            break;
                        case 4:
                            Console.WriteLine($"Current System Clock: {s_bl.Admin.GetClock()}");
                            break;
                        case 5:
                            Console.WriteLine($"Current Risk Time Range: {s_bl.Admin.GetRiskRange()}");
                            break;
                        case 6:
                            Console.Write("Enter new risk time range (hh:mm:ss): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan timeRange))
                            {
                                s_bl.Admin.SetRiskRange(timeRange);
                                Console.WriteLine("Risk time range updated.");
                            }
                            else
                            {
                                throw new FormatException("Invalid time format. Please use hh:mm:ss.");
                            }
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        static void VolunteerMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Volunteer Management ---");
                Console.WriteLine("1. Login volunteer to system");
                Console.WriteLine("2. Get Volunteers List");
                Console.WriteLine("3. Get (Filter/Sort) volunteers List");
                Console.WriteLine("4. Get Volunteer Details by ID");
                Console.WriteLine("5. Add Volunteer");
                Console.WriteLine("6. Delete Volunteer");
                Console.WriteLine("7. Update Volunteer");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    throw new FormatException("The volunteer menu choice is not valid.");
                switch (choice)
                {
                    case 1:
                        try
                        {
                            Console.WriteLine("Please log in.");
                            Console.Write("Enter Username: ");
                            string username = Console.ReadLine()!;

                            Console.Write("Enter Password: ");
                            string password = Console.ReadLine()!;

                            BO.Role userRole = s_bl.Volunteer.LoginVolunteerToSystem(username, password);
                            Console.WriteLine($"Login successful! Your role is: {userRole}");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 2:
                        try
                        {
                            foreach (var volunteer in s_bl.Volunteer.GetVolunteersList())
                            {
                                Console.WriteLine(volunteer);
                            }
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 3:
                        try
                        {
                            bool? isActive;
                            BO.VolunteerSortOption? sortBy;
                            GetVolunteerFilterAndSortCriteria(out isActive, out sortBy);
                            var volunteersList = s_bl.Volunteer.GetVolunteersList(isActive, sortBy);
                            if (volunteersList != null)
                                foreach (var volunteer in volunteersList)
                                    Console.WriteLine(volunteer);
                            else
                                Console.WriteLine("No volunteers found matching the criteria.");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 4:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int volunteerId))
                            {
                                var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                                Console.WriteLine(volunteer);
                            }
                            else
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;
                    case 5:
                        try
                        {
                            Console.WriteLine("Enter Volunteer details:");
                            Console.Write("ID: ");
                            if (int.TryParse(Console.ReadLine(), out int id))
                            {
                                BO.Volunteer volunteer = CreateVolunteer(id);
                                s_bl.Volunteer.AddVolunteer(volunteer);
                                Console.WriteLine("Volunteer created successfully!");
                            }
                            else
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;

                    case 6:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int vId))
                            {
                                s_bl.Volunteer.DeleteVolunteer(vId);
                                Console.WriteLine("Volunteer Deleted.");
                            }
                            else
                            {
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                            }
                        }
                        catch (Exception ex)
                        {
                            HandleException(ex);
                        }
                        break;

                    case 7:
                        UpdateVolunteer();
                        break;

                    case 0:
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }

            }
        }
        public static void GetVolunteerFilterAndSortCriteria(out bool? isActive, out BO.VolunteerSortOption? sortBy)
        {
            isActive = null;
            sortBy = null;

            Console.WriteLine("Is the volunteer active? (yes/no or leave blank for null): ");
            string activeInput = Console.ReadLine();

            if (!string.IsNullOrEmpty(activeInput))
            {
                if (activeInput.Equals("yes", StringComparison.OrdinalIgnoreCase))
                    isActive = true;
                else if (activeInput.Equals("no", StringComparison.OrdinalIgnoreCase))
                    isActive = false;
                else
                    Console.WriteLine("Invalid input for active status. Defaulting to null.");
            }

            Console.WriteLine("Choose how to sort the volunteers by: ");
            Console.WriteLine("1. ByName");
            Console.WriteLine("2. ByCompletedCalls");
            Console.WriteLine("Select sorting option by number: ");
            string sortInput = Console.ReadLine();

            if (int.TryParse(sortInput, out int sortOption))
            {
                switch (sortOption)
                {
                    case 1:
                        sortBy = BO.VolunteerSortOption.ByName;
                        break;
                    case 2:
                        sortBy = BO.VolunteerSortOption.ByCompletedCalls;
                        break;
                    default:
                        Console.WriteLine("Invalid selection. Defaulting to sorting by ID.");
                        break;
                }
            }
            else
            {
                throw new FormatException("Invalid input for sorting option. Defaulting to sorting by ID.");
            }
        }
        static BO.Volunteer CreateVolunteer(int requesterId)
        {

            Console.Write("Full Name: ");
            string? name = Console.ReadLine();

            Console.Write("Phone Number: ");
            string? phoneNumber = Console.ReadLine();

            Console.Write("Email: ");
            string? email = Console.ReadLine();

            Console.Write("IsActive? (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool active))
                throw new FormatException("Invalid input for IsActive.");

            Console.WriteLine("Please enter Role: 'Manager' or 'Volunteer'.");
            if (!Enum.TryParse(Console.ReadLine(), out BO.Role role))
                throw new FormatException("Invalid role.");

            Console.Write("Password: ");
            string? password = Console.ReadLine();

            Console.Write("Full Address: ");
            string? address = Console.ReadLine();


            Console.Write("Max Distance For Call: ");
            if (!double.TryParse(Console.ReadLine(), out double MaxDistanceForCall))
                throw new FormatException("Invalid Max Distance For Call format.");

            Console.Write("Distance Type (Air, Drive or Walk): ");
            if (!Enum.TryParse(Console.ReadLine(), true, out BO.DistanceType myDistanceType))
                throw new FormatException("Invalid distance type.");

            return new BO.Volunteer
            {
                Id = requesterId,
                FullName = name,
                PhoneNumber = phoneNumber,
                Email = email,
                Password = password,
                CurrentFullAddress = address,
                Latitude = null,
                Longitude = null,
                Role = role,
                IsActive = active,
                MaxDistanceForCall = MaxDistanceForCall,
                DistanceType = myDistanceType,
            };
        }
        static void UpdateVolunteer()
        {
            try
            {
                Console.Write("Enter requester ID: ");
                if (int.TryParse(Console.ReadLine(), out int requesterId))
                {
                    BO.Volunteer boVolunteer = CreateVolunteer(requesterId);
                    s_bl.Volunteer.UpdateVolunteerDetails(requesterId, boVolunteer);
                    Console.WriteLine("Volunteer updated successfully.");
                }
                else
                    throw new FormatException("Invalid input. Volunteer ID must be a number.");

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }


        static void CallMenu()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("\n--- Call Management ---");
                    Console.WriteLine("1. Get call quantities by status");
                    Console.WriteLine("2. Get Closed Calls Handled By Volunteer");
                    Console.WriteLine("3. Show All Calls");
                    Console.WriteLine("4. Get Call Details by ID");
                    Console.WriteLine("5. Add Call");
                    Console.WriteLine("6. Delete Call");
                    Console.WriteLine("7. Update Call");
                    Console.WriteLine("8. Get Open Calls For Volunteer");
                    Console.WriteLine("9. Cancel Call Assignment");
                    Console.WriteLine("10. Mark Call As Completed");
                    Console.WriteLine("11. Select Call For Treatment");
                    Console.WriteLine("0. Back");
                    Console.Write("Choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                        throw new FormatException("The call menu choice is not valid.");

                    switch (choice)
                    {
                        case 1:
                            try
                            {
                                IEnumerable<int> callQuantities = s_bl.Call.GetCallQuantitiesByStatus();

                                Console.WriteLine("Call quantities by status:");

                                int index = 0;
                                foreach (BO.CallStatus status in Enum.GetValues(typeof(BO.CallStatus)))
                                {
                                    Console.WriteLine($"{status}: {callQuantities.ElementAt(index)}");
                                    index++;
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 2:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                                {
                                    Console.WriteLine("Enter Filter Type ( CallType, OpenTime, MaxEndTime, Status) or press Enter to skip:");
                                    string? FilterTypeInput = Console.ReadLine();
                                    BO.FilterAndSortByFields? FilterType = Enum.TryParse(FilterTypeInput, out BO.FilterAndSortByFields parsedCallType) ? parsedCallType : null;
                                    Console.WriteLine("Enter Sort Field ( CallType, OpenTime, MaxEndTime, Status) or press Enter to skip:");
                                    string? sortFieldInput = Console.ReadLine();
                                    BO.FilterAndSortByFields? sortField = Enum.TryParse(sortFieldInput, out BO.FilterAndSortByFields parsedSortField) ? parsedSortField : null;

                                    var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId, FilterType, sortField);

                                    Console.WriteLine("\nClosed Calls Handled By Volunteer:");
                                    foreach (var call in closedCalls)
                                    {
                                        Console.WriteLine(call);
                                    }
                                }
                                else
                                {
                                    throw new BO.BlInvalidInputException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 3:
                            try
                            {
                                Console.WriteLine("Enter Filter Type ( CallId, CallType, OpenTime, RestTimeForCall, LastVolunteerName, RestTimeForTreatment, Status) or press Enter to skip:");
                                string? filterFieldInput = Console.ReadLine();
                                BO.CallInListFields? filterField = Enum.TryParse(filterFieldInput, out BO.CallInListFields parsedFilterField) ? parsedFilterField : null;

                                object? filterValue = null;
                                if (filterField.HasValue)
                                {
                                    Console.WriteLine("Enter filter value:");
                                    filterValue = Console.ReadLine();
                                }

                                Console.WriteLine("Enter sort field ( CallId, CallType, OpenTime, RestTimeForCall, LastVolunteerName, RestTimeForTreatment, Status) or press Enter to skip:");
                                string? sortFieldInput = Console.ReadLine();
                                BO.CallInListFields? sortField = Enum.TryParse(sortFieldInput, out BO.CallInListFields parsedSortField) ? parsedSortField : null;

                                var callList = s_bl.Call.GetCallsList(filterField, filterValue, sortField);

                                foreach (var call in callList)
                                    Console.WriteLine(call);
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 4:
                            try
                            {
                                Console.Write("Enter Call ID: ");
                                if (int.TryParse(Console.ReadLine(), out int callId))
                                {
                                    var call = s_bl.Call.GetCallDetails(callId);
                                    Console.WriteLine(call);
                                }
                                else
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 5:
                            try
                            {
                                Console.WriteLine("Enter Call details:");
                                BO.Call call = CreateCall();
                                s_bl.Call.AddCall(call);
                                Console.WriteLine("Call created successfully!");
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            ;
                            break;
                        case 6:
                            try
                            {
                                Console.Write("Enter Call ID: ");
                                if (int.TryParse(Console.ReadLine(), out int cId))
                                {
                                    s_bl.Call.DeleteCall(cId);
                                    Console.WriteLine("Call Deleted.");
                                }
                                else
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 7:
                            UpdateCall();
                            break;
                        case 8:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                                {
                                    Console.WriteLine("Enter Filter Type ( CallType, OpenTime, MaxEndTime, Status) or press Enter to skip:");
                                    string? FilterTypeInput = Console.ReadLine();
                                    BO.FilterAndSortByFields? FilterType = Enum.TryParse(FilterTypeInput, out BO.FilterAndSortByFields parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field ( CallType, OpenTime, MaxEndTime, Status) or press Enter to skip:");
                                    string? sortFieldInput = Console.ReadLine();
                                    BO.FilterAndSortByFields? sortField = Enum.TryParse(sortFieldInput, out BO.FilterAndSortByFields parsedSortField) ? parsedSortField : null;

                                    var openCalls = s_bl.Call.GetOpenCallsByVolunteer(volunteerId, FilterType, sortField);

                                    Console.WriteLine("\nOpen Calls Available for Volunteer:");
                                    foreach (var call in openCalls)
                                    {
                                        Console.WriteLine(call);
                                    }
                                }
                                else
                                {
                                    throw new BO.BlInvalidInputException("Invalid input. Volunteer ID must be a number.");
                                }
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 9:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                    throw new BO.BlInvalidInputException("Invalid input. Volunteer ID must be a number.");

                                Console.Write("Enter call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int assignmentId))
                                    throw new BO.BlInvalidInputException("Invalid input. call ID must be a number.");

                                s_bl.Call.CancelCallAssignment(volunteerId, assignmentId);
                                Console.WriteLine("The call was successfully canceled.");
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 10:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                string? volunteerInput = Console.ReadLine();
                                if (!int.TryParse(volunteerInput, out int volunteerId))
                                {
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
                                }

                                Console.Write("Enter Assignment ID: ");
                                string? assignmentInput = Console.ReadLine();
                                if (!int.TryParse(assignmentInput, out int assignmentId))
                                {
                                    throw new FormatException("Invalid input. Assignment ID must be a number.");
                                }

                                s_bl.Call.MarkCallAsCompleted(volunteerId, assignmentId);

                                Console.WriteLine("Call completion updated successfully!");
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 11:
                            try
                            {
                                Console.Write("Enter Volunteer ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int volunteerId))
                                    throw new FormatException("Invalid input. Volunteer ID must be a number.");

                                Console.Write("Enter Call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int callId))
                                    throw new FormatException("Invalid input. Call ID must be a number.");

                                s_bl.Call.SelectCallForTreatment(volunteerId, callId);
                                Console.WriteLine("The call has been successfully assigned to the volunteer.");
                            }
                            catch (Exception ex)
                            {
                                HandleException(ex);
                            }
                            break;
                        case 0:
                            return;
                        default:
                            Console.WriteLine("Invalid choice. Try again.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        static BO.Call CreateCall()
        {
            Console.WriteLine("Enter the call type (0 for Emergency, 1 for Equipment, 2 for Doctor, 3 for Training):");
            if (!Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
            {
                throw new FormatException("Invalid call type.");
            }

            Console.WriteLine("Enter the description:");
            string description = Console.ReadLine();

            Console.WriteLine("Enter the address:");
            string address = Console.ReadLine();

            Console.WriteLine("Enter the max End time (yyyy-mm-dd) or leave empty:");
            string maxEndTimeInput = Console.ReadLine();
            DateTime? maxEndTime = string.IsNullOrEmpty(maxEndTimeInput) ? null : DateTime.Parse(maxEndTimeInput);

            Console.WriteLine("Enter the status (0 for Open, 1 for InProcessing, 2 for Closed, 3 for Expired, 4 for OpenAtRisk ):");
            if (!Enum.TryParse(Console.ReadLine(), out BO.CallStatus status))
            {
                throw new FormatException("Invalid status.");
            }

            return new BO.Call
            {
                CallType = callType,
                Description = description,
                FullAddress = address,
                Latitude = 0,
                Longitude = 0,
                OpenTime = s_bl.Admin.GetClock(),
                MaxEndTime = maxEndTime
            };


        }
        static void UpdateCall()
        {
            Console.Write("Enter Call ID: ");
            int.TryParse(Console.ReadLine(), out int callId);
            Console.Write("Enter New Description (optional) : ");
            string description = Console.ReadLine();
            Console.Write("Enter New Full Address (optional) : ");
            string address = Console.ReadLine();
            Console.Write("Enter New Call Type (optional) : ");
            BO.CallType? callType = Enum.TryParse(Console.ReadLine(), out BO.CallType parsedType) ? parsedType : (BO.CallType?)null;
            Console.Write("Enter New End Time (hh:mm , (optional)): ");
            TimeSpan? maxEndTime = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan parsedTime) ? parsedTime : (TimeSpan?)null;
            try
            {
                var callToUpdate = s_bl.Call.GetCallDetails(callId);
                if (callToUpdate == null)
                    throw new BO.BlDoesNotExistException($"Call with ID{callId} does not exist!");
                var newUpdatedCall = new BO.Call
                {
                    Id = callId,
                    Description = !string.IsNullOrWhiteSpace(description) ? description : callToUpdate.Description,
                    FullAddress = !string.IsNullOrWhiteSpace(address) ? address : callToUpdate.FullAddress,
                    OpenTime = callToUpdate.OpenTime,
                    MaxEndTime = (maxEndTime.HasValue ? DateTime.Now.Date + maxEndTime.Value : callToUpdate.MaxEndTime),
                    CallType = callType ?? callToUpdate.CallType
                };
                s_bl.Call.UpdateCallDetails(newUpdatedCall);
                Console.WriteLine("Call updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.GetType().Name}, Message: {ex.Message}");
            }
        }
        static void HandleException(Exception ex)
        {
            switch (ex)
            {
                case BO.BlDoesNotExistException ex1:
                    Console.WriteLine($"Exception: {ex1.GetType().Name}, Message: {ex1.Message}");
                    break;
                case BO.BlAlreadyExistsException ex2:
                    Console.WriteLine($"Exception: {ex2.GetType().Name}, Message: {ex2.Message}");
                    break;
                case BO.BlFormatException ex3:
                    Console.WriteLine($"Exception: {ex3.GetType().Name}, Message: {ex3.Message}");
                    break;
                case BO.BlNullReferenceException ex4:
                    Console.WriteLine($"Exception: {ex4.GetType().Name}, Message: {ex4.Message}");
                    break;
                case BO.BlXMLFileLoadCreateException ex5:
                    Console.WriteLine($"Exception: {ex5.GetType().Name}, Message: {ex5.Message}");
                    if (ex5.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex5.InnerException.Message}");
                    }
                    break;
                case BO.BlNotImplementedException ex6:
                    Console.WriteLine($"Exception: {ex6.GetType().Name}, Message: {ex6.Message}");
                    break;
                case BO.BlArgumentNullException ex7:
                    Console.WriteLine($"Exception: {ex7.GetType().Name}, Message: {ex7.Message}");
                    break;
                case BO.BlInvalidTimeException ex8:
                    Console.WriteLine($"Exception: {ex8.GetType().Name}, Message: {ex8.Message}");
                    break;
                case BO.BlInvalidInputException ex9:
                    Console.WriteLine($"Exception: {ex9.GetType().Name}, Message: {ex9.Message}");
                    break;
                case BO.BlInvalidOperationException ex10:
                    Console.WriteLine($"Exception: {ex10.GetType().Name}, Message: {ex10.Message}");
                    break;
                case BO.BlAuthorizationException ex11:
                    Console.WriteLine($"Unauthorized Access: {ex11.Message}");
                    break;
                case FormatException:
                    Console.WriteLine("Input format is incorrect. Please try again.");
                    break;
                case Exception exe:
                    Console.WriteLine($"An unexpected error occurred: {exe.Message}");
                    break;
            }
        }

    }

}


