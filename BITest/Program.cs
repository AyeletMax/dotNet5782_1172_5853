using System;
using BlApi;
using BO;

using DO;
///לבדוק מה עם כל הזריקות שיש פה
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
            catch (BO.BlInvalidFormatException ex)
            {
                Console.WriteLine("The sub menu choice is not valid.", ex);
            }
            catch (BO.BlGeneralDatabaseException ex)
            {
                Console.WriteLine("A custom error occurred in Initialization.", ex);
            }
        }

        static void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Administration ---");
                Console.WriteLine("1. Reset Database");
                Console.WriteLine("2. Initialize Database");
                Console.WriteLine("3. Advance Clock");
                Console.WriteLine("4. Show Clock");
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
                                s_bl.Admin.AdvanceClock(timeUnit);
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
                            Console.WriteLine($"Current Risk Time Range: {s_bl.Admin.GetMaxRange()}");
                            break;
                        case 6:
                            Console.Write("Enter new risk time range (hh:mm:ss): ");
                            if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan timeRange))
                            {
                                s_bl.Admin.SetMaxRange(timeRange);
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
                catch (BO.BlInvalidFormatException)
                {
                    Console.WriteLine("Invalid time format.");
                }
                catch (BO.BlGeneralDatabaseException ex)
                {
                    Console.WriteLine($"A database error occurred: {ex.Message}");
                }
            }
        }

        static void VolunteerMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Volunteer Management ---");
                Console.WriteLine("1. Login");
                Console.WriteLine("2. List Volunteers");
                Console.WriteLine("3. Get Filter/Sort volunteer");
                Console.WriteLine("4. Read Volunteer by ID");
                Console.WriteLine("5. Add Volunteer");
                Console.WriteLine("6. Remove Volunteer");
                Console.WriteLine("7. UpDate Volunteer");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    //האם אפשר לעשות פה כזאת זריקה?
                    throw new FormatException("The volunteer menu choice is not valid.");

                switch (choice)
                {
                    case 1:
                        try
                        {
                            Console.WriteLine("Please log in.");
                            Console.Write("Username: ");
                            string username = Console.ReadLine()!;

                            Console.Write("Console.Write(\"Enter Password (must be at least 8 characters, contain upper and lower case letters, a digit, and a special character): \");");
                            string password = Console.ReadLine()!;

                            BO.Role userRole = s_bl.Volunteer.Login(username, password);
                            Console.WriteLine($"Login successful! Your role is: {userRole}");
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"System Error: {ex.Message}");
                        }
                        break;
                    case 2:
                        try
                        {
                            foreach (var volunteer in s_bl.Volunteer.GetVolunteersList())
                                Console.WriteLine(volunteer);
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"System Error: {ex.Message}");
                        }
                        break;
                    case 3:
                        try { 
                        bool? isActive;
                        BO.VolunteerSortField? sortBy;
                        GetVolunteerFilterAndSortCriteria(out isActive, out sortBy);
                        var volunteersList = s_bl.Volunteer.GetVolunteersList(isActive, sortBy);
                        if (volunteersList != null)
                            foreach (var volunteer in volunteersList)
                                Console.WriteLine(volunteer);
                        else
                            Console.WriteLine("No volunteers found matching the criteria.");
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"System Error: {ex.Message}");
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
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;
                    case 5:
                        try
                        {
                            Console.WriteLine("Enter Volunteer details:");
                            Console.Write("ID: ");
                            if (int.TryParse(Console.ReadLine(), out int id)) { 
                                BO.Volunteer volunteer = CreateVolunteer();
                                volunteer.Id = id;
                                s_bl.Volunteer.AddVolunteer(volunteer);
                                Console.WriteLine("Volunteer created successfully!");
                            }
                            else
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
                        }
                        catch (BO.BlAlreadyExistsException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlInvalidFormatException ex)
                        {
                            Console.WriteLine($"Input Error: {ex.Message}");
                        }
                        catch (BO.BlApiRequestException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeolocationNotFoundException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;
                    case 6:
                        try
                        {
                            Console.Write("Enter Volunteer ID: ");
                            if (int.TryParse(Console.ReadLine(), out int vId))
                            {
                                s_bl.Volunteer.DeleteVolunteer(vId);
                                Console.WriteLine("Volunteer removed.");
                            }
                            else
                            {
                                throw new FormatException("Invalid input. Volunteer ID must be a number."); 
                            }
                        }
                        catch (BO.BlDoesNotExistException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        catch (BO.BlGeneralDatabaseException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                        }
                        break;
                    case 7:
                        UpDateVolunteer();
                        break;
                    case 0:
                        return;
                    default:
                        //כנל לבדיקה האם לזרוק פה
                        Console.WriteLine("Invalid choice. Try again.");
                        break;
                }
            }
        }
        public static void GetVolunteerFilterAndSortCriteria(out bool? isActive, out BO.VolunteerSortField? sortBy)
        {
            isActive = null;
            sortBy = null;

            try
            {

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
                Console.WriteLine("1. ID");
                Console.WriteLine("2. Name");
                Console.WriteLine("3. Total Responses Handled");
                Console.WriteLine("4. Total Responses Cancelled");
                Console.WriteLine("5. Total Expired Responses");
                Console.WriteLine("6. Sum of Calls");
                Console.WriteLine("7. Sum of Cancellations");
                Console.WriteLine("8. Sum of Expired Calls");
                Console.WriteLine("Select sorting option by number: ");
                string sortInput = Console.ReadLine();

                if (int.TryParse(sortInput, out int sortOption))
                {
                    switch (sortOption)
                    {
                        case 1:
                            sortBy = BO.VolunteerSortField.Id;
                            break;
                        case 2:
                            sortBy = BO.VolunteerSortField.Name;
                            break;
                        case 3:
                            sortBy = BO.VolunteerSortField.TotalResponsesHandled;
                            break;
                        case 4:
                            sortBy = BO.VolunteerSortField.TotalResponsesCancelled;
                            break;
                        case 5:
                            sortBy = BO.VolunteerSortField.TotalExpiredResponses;
                            break;
                        case 6:
                            sortBy = BO.VolunteerSortField.SumOfCalls;
                            break;
                        case 7:
                            sortBy = BO.VolunteerSortField.SumOfCancellation;
                            break;
                        case 8:
                            sortBy = BO.VolunteerSortField.SumOfExpiredCalls;
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
            catch (BO.BlGeneralDatabaseException ex)
            {
                Console.WriteLine($"Exception: {ex.GetType().Name}");
                Console.WriteLine($"Message: {ex.Message}");
            }
        }
        //מה לעשות עם כל הTRY ועם הזריקות
        static BO.Volunteer CreateVolunteer()
        {
 
             Console.Write("Name: ");
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

             Console.Write("Address: ");
             string? address = Console.ReadLine();

             Console.WriteLine("Enter location details:");
             Console.Write("Latitude: ");
             if (!double.TryParse(Console.ReadLine(), out double latitude))
                throw new FormatException("Invalid latitude format.");

             Console.Write("Longitude: ");
             if (!double.TryParse(Console.ReadLine(), out double longitude))
                throw new FormatException("Invalid longitude format.");

             Console.Write("Largest Distance: ");
             if (!double.TryParse(Console.ReadLine(), out double largestDistance))
                throw new FormatException("Invalid largest distance format.");

             Console.Write("Distance Type (Air, Drive or Walk): ");
             if (!Enum.TryParse(Console.ReadLine(), true, out BO.DistanceType myDistanceType))
                throw new FormatException("Invalid distance type.");

             return new BO.Volunteer
             {
                 Name = name,
                 Phone = phoneNumber,
                 Email = email,
                 Active = active,
                 MyRole = role,
                 Password = password,
                 Address = address,
                 Latitude = latitude,
                 Longitude = longitude,
                 LargestDistance = largestDistance,
                 MyDistanceType = myDistanceType,
                 TotalCallsHandled = 0,
                 TotalCallsCancelled = 0,
                 TotalExpiredCallsChosen = 0,
                 CurrentCallInProgress = null
             };

             
           
          
        }
        //לדעת לטפל בזריקות עם כל הTRY
        static void UpDateVolunteer()
        {

            //מה עושים עם כל אלה בעדכון?
            //TotalCallsHandled = 0,
            //     TotalCallsCancelled = 0,
            //     TotalExpiredCallsChosen = 0,
            try
            {
                BO.Volunteer boVolunteer = CreateVolunteer();
                Console.Write("Enter requester ID: ");
                if (int.TryParse(Console.ReadLine(), out int requesterId))
                {
                    s_bl.Volunteer.UpdateVolunteer(requesterId, boVolunteer);
                    Console.WriteLine("Volunteer updated successfully.");
                }
                else
                    throw new FormatException("Invalid input. Volunteer ID must be a number.");
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BO.BlUnauthorizedAccessException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BO.BlInvalidFormatException ex)
            {
                Console.WriteLine(ex);
            }
            catch (BO.BlGeneralDatabaseException ex)
            {
                Console.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An unexpected error occurred: " + ex.Message);
            }
        }


        static void CallMenu()
        {
            try
            {

                while (true)
                {
                    Console.WriteLine("\n--- Call Management ---");
                    Console.WriteLine("1. Show All Calls");
                    Console.WriteLine("2. Read Call by ID");
                    Console.WriteLine("3. Add Call");
                    Console.WriteLine("4. Remove Call");
                    Console.WriteLine("0. Back");
                    Console.Write("Choose an option: ");

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                        throw new FormatException("The call menu choice is not valid.");

                    switch (choice)
                    {
                        case 1:
                            foreach (var call in s_bl.Call.GetCallList())
                                Console.WriteLine(call);
                            break;
                        case 2:
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
                            break;
                        case 3:
                            //צריך לעשות את כל השדות
                            Console.Write("Enter Call Type (Emergency, Assistance, etc.): ");
                            if (Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
                            {
                                Console.Write("Enter Description: ");
                                string description = Console.ReadLine()!;
                                Console.Write("Enter Address: ");
                                string address = Console.ReadLine()!;

                                Console.Write("Enter Latitude: ");
                                if (double.TryParse(Console.ReadLine(), out double latitude))
                                {
                                    Console.Write("Enter Longitude: ");
                                    if (double.TryParse(Console.ReadLine(), out double longitude))
                                    {
                                        var call = new BO.Call
                                        {
                                            MyCallType = callType,
                                            VerbalDescription = description,
                                            Address = address,
                                            Latitude = latitude,
                                            Longitude = longitude,
                                            OpenTime = DateTime.Now,
                                            //לבדוק מה קורה עם הסטטוס של קריאה
                                            //MyStatus = BO.CallStatus.Open
                                        };
                                        s_bl.Call.AddCall(call);
                                        Console.WriteLine("Call added.");
                                    }
                                    else
                                    {
                                        throw new FormatException("Invalid longitude format.");
                                    }
                                }
                                else
                                {
                                    throw new FormatException("Invalid latitude format.");
                                }
                            }
                            else
                            {
                                throw new FormatException("Invalid call type.");
                            }

                            break;
                        case 4:
                            Console.Write("Enter Call ID: ");
                            if (int.TryParse(Console.ReadLine(), out int cId))
                            {
                                s_bl.Call.DeleteCall(cId);
                                Console.WriteLine("Call removed.");
                            }
                            else
                            {
                                throw new FormatException("Invalid input. Volunteer ID must be a number.");
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
    }
}
