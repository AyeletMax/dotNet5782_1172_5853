using System;
using BlApi;
using BO;
using DO;
namespace BlTest
{
    /// <summary>
    /// The Program class serves as the main entry point for the BL Test System.
    /// It provides a console-based interface for administrators and volunteers to interact with the system.
    /// </summary>
    class Program
    {

        // <summary>
        /// Static instance of the business logic layer (BL) obtained from the factory.
        /// </summary>
        static readonly IBl s_bl = Factory.Get();

        /// <summary>
        /// Main method that presents a menu to the user and handles their selection.
        /// </summary>
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


        /// <summary>
        /// Displays the administration menu and handles the corresponding actions.
        /// </summary>
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
                catch (Exception ex) 
                { 
                    HandleException(ex);
                }
            }
        }

        /// <summary>
        /// Displays the volunteer management menu and handles user actions.
        /// </summary>
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
                    throw new FormatException("The volunteer menu choice is not valid.");
                switch (choice)
                {
                    case 1:
                        try
                        {
                            Console.WriteLine("Please log in.");
                            Console.Write("Username: ");
                            int username = Console.Read()!;

                            Console.Write("Enter Password (must be at least 8 characters, contain upper and lower case letters, a digit, and a special character): ");
                            string password = Console.ReadLine()!;

                            BO.Role userRole = s_bl.Volunteer.Login(username, password);
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
                                Console.WriteLine(volunteer);
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
                            BO.VolunteerSortField? sortBy;
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
                                Console.WriteLine("Volunteer removed.");
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
                        UpDateVolunteer();
                        break;

                    case 0:
                        return;

                    default:
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
                        //case 6:
                        //    sortBy = BO.VolunteerSortField.SumOfCalls;
                        //    break;
                        //case 7:
                        //    sortBy = BO.VolunteerSortField.SumOfCancellation;
                        //    break;
                        //case 8:
                        //    sortBy = BO.VolunteerSortField.SumOfExpiredCalls;
                        //    break;
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
        static BO.Volunteer CreateVolunteer(int requesterId)
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

             Console.Write("Largest Distance: ");
             if (!double.TryParse(Console.ReadLine(), out double largestDistance))
                throw new FormatException("Invalid largest distance format.");

             Console.Write("Distance Type (Air, Drive or Walk): ");
             if (!Enum.TryParse(Console.ReadLine(), true, out BO.DistanceType myDistanceType))
                throw new FormatException("Invalid distance type.");
             
            return new BO.Volunteer
            {
                Id = requesterId,
                Name = name,
                Phone = phoneNumber,
                Email = email,
                Active = active,
                MyRole = role,
                Password = password,
                Address = address,
                LargestDistance = largestDistance,
                MyDistanceType = myDistanceType,
            };  
        }
        static void UpDateVolunteer()
        {
            try
            {
                Console.Write("Enter requester ID: ");
                if (int.TryParse(Console.ReadLine(), out int requesterId))
                {
                    BO.Volunteer boVolunteer = CreateVolunteer(requesterId);
                    s_bl.Volunteer.UpdateVolunteer(requesterId, boVolunteer);
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

        /// <summary>
        /// Displays the call management menu and processes user choices.
        /// </summary>
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
                    Console.WriteLine("4. Read Call by ID");
                    Console.WriteLine("5. Add Call");
                    Console.WriteLine("6. Remove Call");
                    Console.WriteLine("7. Update Call");
                    Console.WriteLine("8. Get Open Calls For Volunteer");
                    Console.WriteLine("9. Mark Call As Canceled");
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
                                int[] callQuantities = s_bl.Call.GetCallQuantitiesByStatus();
                                Console.WriteLine("Call quantities by status:");

                                foreach (BO.Status status in Enum.GetValues(typeof(BO.Status)))
                                {
                                    Console.WriteLine($"{status}: {callQuantities[(int)status]}");
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
                                    Console.WriteLine("Enter Call Type filter (or press Enter to skip):");
                                    string? callTypeInput = Console.ReadLine();
                                    BO.CallType? callTypeFilter = Enum.TryParse(callTypeInput, out BO.CallType parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                                    string? sortFieldInput = Console.ReadLine();
                                    BO.ClosedCallInListFields? sortField = Enum.TryParse(sortFieldInput, out BO.ClosedCallInListFields parsedSortField) ? parsedSortField : null;

                                    var closedCalls = s_bl.Call.GetClosedCallsByVolunteer(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nClosed Calls Handled By Volunteer:");
                                    foreach (var call in closedCalls)
                                    {
                                        Console.WriteLine(call);
                                    }
                                }
                                else
                                {
                                    throw new BO.BlInvalidFormatException("Invalid input. Volunteer ID must be a number.");
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
                                Console.WriteLine("Enter sort field (CallId, CallType, OpenTime, TimeRemainingToCall, LastVolunteer, CompletionTime, MyStatus, TotalAllocations) or press Enter to skip:");
                                string? filterFieldInput = Console.ReadLine();
                                BO.CallInListFields? filterField = Enum.TryParse(filterFieldInput, out BO.CallInListFields parsedFilterField) ? parsedFilterField : null;

                                object? filterValue = null;
                                if (filterField.HasValue)
                                {
                                    Console.WriteLine("Enter filter value:");
                                    filterValue = Console.ReadLine();
                                }

                                Console.WriteLine("Enter sort field (CallId, CallType, OpenTime, TimeRemainingToCall, LastVolunteer, CompletionTime, MyStatus, TotalAllocations) or press Enter to skip:");
                                string? sortFieldInput = Console.ReadLine();
                                BO.CallInListFields? sortField = Enum.TryParse(sortFieldInput, out BO.CallInListFields parsedSortField) ? parsedSortField : null;

                                var callList = s_bl.Call.GetCallList(filterField, filterValue, sortField);

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
                                Console.Write("ID: ");
                                if (int.TryParse(Console.ReadLine(), out int id))
                                {
                                    BO.Call call = CreateCall(id);
                                    s_bl.Call.AddCall(call);
                                    Console.WriteLine("Call created successfully!");
                                }
                                else
                                    throw new FormatException("Invalid input. Cll ID must be a number.");
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
                                    Console.WriteLine("Call removed.");
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
                            UpDateCall();
                            break;
                        case 8:
                            try {
                                Console.Write("Enter Volunteer ID: ");
                                if (int.TryParse(Console.ReadLine(), out int volunteerId))
                                {
                                    Console.WriteLine("Enter Call Type filter (or press Enter to skip):");
                                    string? callTypeInput = Console.ReadLine();
                                    BO.CallType? callTypeFilter = Enum.TryParse(callTypeInput, out BO.CallType parsedCallType) ? parsedCallType : null;

                                    Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                                    string? sortFieldInput = Console.ReadLine();
                                    BO.OpenCallInListFields? sortField = Enum.TryParse(sortFieldInput, out BO.OpenCallInListFields parsedSortField) ? parsedSortField : null;

                                    var openCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId, callTypeFilter, sortField);

                                    Console.WriteLine("\nOpen Calls Available for Volunteer:");
                                    foreach (var call in openCalls)
                                    {
                                        Console.WriteLine(call);
                                    }
                                }
                                else
                                {
                                    throw new BlInvalidFormatException("Invalid input. Volunteer ID must be a number.");
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
                                    throw new BlInvalidFormatException("Invalid input. Volunteer ID must be a number.");

                                Console.Write("Enter call ID: ");
                                if (!int.TryParse(Console.ReadLine(), out int assignmentId))
                                    throw new BlInvalidFormatException("Invalid input. call ID must be a number.");

                                s_bl.Call.UpdateCallCancellation(volunteerId, assignmentId);
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

                                s_bl.Call.UpdateCallCompletion(volunteerId, assignmentId);

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
        static BO.Call CreateCall(int id)
        {
                Console.WriteLine("Enter the call type (0 for None, 1 for MusicPerformance, 2 for MusicTherapy, 3 for SingingAndEmotionalSupport, 4 for GroupActivities, 5 for PersonalizedMusicCare):");
                if (!Enum.TryParse(Console.ReadLine(), out BO.CallType callType))
                {
                    throw new FormatException("Invalid call type.");
                }

                Console.WriteLine("Enter the verbal description:");
                string verbalDescription = Console.ReadLine();

                Console.WriteLine("Enter the address:");
                string address = Console.ReadLine();

 

                Console.WriteLine("Enter the max finish time (yyyy-mm-dd) or leave empty:");
                string maxFinishTimeInput = Console.ReadLine();
                DateTime? maxFinishTime = string.IsNullOrEmpty(maxFinishTimeInput) ? null : DateTime.Parse(maxFinishTimeInput);

                Console.WriteLine("Enter the status (0 for InProgress, 1 for AtRisk, 2 for InProgressAtRisk, 3 for Opened, 4 for Closed, 5 for Expired):");
                if (!Enum.TryParse(Console.ReadLine(), out Status status))
                {
                    throw new FormatException("Invalid status.");
                }

            return new BO.Call
            {
                Id = id,
                MyCallType = callType,
                VerbalDescription = verbalDescription,
                Address = address,
                Latitude = 0,
                Longitude = 0,
                OpenTime = s_bl.Admin.GetClock()
            };
            
           
        }
        static void UpDateCall()
        {
            Console.Write("Enter Call ID: ");
            int.TryParse(Console.ReadLine(), out int callId);
            Console.Write("Enter New Description (optional) : ");
            string description = Console.ReadLine();
            Console.Write("Enter New Full Address (optional) : ");
            string address = Console.ReadLine();
            Console.Write("Enter Call Type (optional) : ");
            BO.CallType? callType = Enum.TryParse(Console.ReadLine(), out BO.CallType parsedType) ? parsedType : (BO.CallType?)null;
            Console.Write("Enter Max Finish Time (hh:mm , (optional)): ");
            TimeSpan? maxFinishTime = TimeSpan.TryParse(Console.ReadLine(), out TimeSpan parsedTime) ? parsedTime : (TimeSpan?)null;
            try
            {
                var callToUpdate = s_bl.Call.GetCallDetails(callId);
                if (callToUpdate == null)
                    throw new BO.BlDoesNotExistException($"Call with ID{callId} does not exist!");
                var newUpdatedCall = new BO.Call
                {
                    Id = callId,
                    VerbalDescription = !string.IsNullOrWhiteSpace(description) ? description : callToUpdate.VerbalDescription,
                    Address = !string.IsNullOrWhiteSpace(address) ? address : /*callToUpdate. FullAddress*/"No Address",
                    OpenTime = callToUpdate.OpenTime,
                    MaxFinishTime = (maxFinishTime.HasValue ? DateTime.Now.Date + maxFinishTime.Value : callToUpdate.MaxFinishTime),
                    MyCallType = callType ?? callToUpdate.MyCallType
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
                case BO.BlInvalidOperationException ex2:
                    Console.WriteLine($"Exception: {ex2.GetType().Name}, Message: {ex2.Message}");
                    break;
                case BO.BlInvalidFormatException ex3:
                    Console.WriteLine($"Exception: {ex3.GetType().Name}, Message: {ex3.Message}");
                    break;
                case BO.BlAlreadyExistsException ex4:
                    Console.WriteLine($"Exception: {ex4.GetType().Name}, Message: {ex4.Message}");
                    break;
                case BO.BlGeneralDatabaseException ex5:
                    Console.WriteLine($"Exception: {ex5.GetType().Name}, Message: {ex5.Message}");
                    if (ex5.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex5.InnerException.Message}");
                    }
                    break;
                case BO.BlGeolocationNotFoundException ex6:
                    Console.WriteLine($"Exception: {ex6.GetType().Name}, Message: {ex6.Message}");
                    break;
                case BO.BlDeletionException ex7:
                    Console.WriteLine($"Exception: {ex7.GetType().Name}, Message: {ex7.Message}");
                    break;
                case BO.BlApiRequestException ex8:
                    Console.WriteLine($"Exception: {ex8.GetType().Name}, Message: {ex8.Message}");
                    break;
                case BO.BlUnauthorizedAccessException ex9:
                    Console.WriteLine($"Unauthorized Access: {ex9.Message}");
                    break;
                case FormatException _:
                    Console.WriteLine("Input format is incorrect. Please try again.");
                    break;
                case Exception ex10:
                    Console.WriteLine($"An unexpected error occurred: {ex10.Message}");
                    break;
            }
        }
    }

}


