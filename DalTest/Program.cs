using Dal;
using DalApi;
using DO;

namespace DalTest
{
    internal class Program
    {
       
        static readonly IDal s_dal = new Dal.DalList(); 
        enum MainMenuChoice
        {
            Exit, Volunteer, Assignments, Calls, Config, InitializeData, ResetDatabase, DisplayAllData
        }
        enum CrudChoice
        {
            Exit, Create, Read, ReadAll, Update, Delete, DeleteAll
        }
        enum ConfigChoice
        {
            Exit, AdvanceClockMinute, AdvanceClockHour, AdvanceClockByDay, AdvanceClockByMonth, AdvanceClockByYear, ShowCurrentClock, ChangeClock, ResetConfig
        }
        static void Main(string[] args)
        {
            try
            {
                mainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        /// <summary>
        /// Displays the main menu, handles user input, and navigates to different menu options.
        /// Executes actions based on the user's choice (e.g., CRUD operations, configuration, database reset).
        /// Loops until the user selects the "Exit" option.
        /// </summary>
        static void mainMenu()
        {
            // Displays menu choices and prompts for user input.
            Console.WriteLine("Enter a number:");
            foreach (MainMenuChoice choice in Enum.GetValues(typeof(MainMenuChoice)))
            {
                Console.WriteLine($"{(int)choice}. {choice}");
            }
            // Reads user input and processes menu actions.
            MainMenuChoice mainMenuChoice;
            Enum.TryParse(Console.ReadLine(), out mainMenuChoice);
            // Loops until "Exit" is selected.
            while (mainMenuChoice is not MainMenuChoice.Exit)
            {
                try
                {
                    // Switches to corresponding action based on user choice.
                    switch (mainMenuChoice)
                    {
                        case MainMenuChoice.Volunteer:
                            CrudMenu("Volunteer");
                            break;
                        case MainMenuChoice.Assignments:
                            CrudMenu("Assignment");
                            break;
                        case MainMenuChoice.Calls:
                            CrudMenu("Call");
                            break;
                        case MainMenuChoice.Config:
                            ShowConfigMenu();
                            break;
                        case MainMenuChoice.InitializeData:
                            Initialization.DO(s_dal);
                            break;
                        case MainMenuChoice.DisplayAllData:
                            try
                            {
                                DisplayAllData();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error displaying all data: {ex.Message}");
                            }
                            break;

                        case MainMenuChoice.ResetDatabase:
                            try
                            {
                                ResetDatabase();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error resetting database: {ex.Message}");
                            }
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
                // Prompts for user input again.
                Console.WriteLine("Enter a number:");
                Enum.TryParse(Console.ReadLine(), out mainMenuChoice);

            }
        }
        /// <summary>
        /// Resets the database by clearing all data and resetting configurations.
        /// </summary>
        private static void ResetDatabase()
        {
            s_dal.Config.Reset();
            s_dal.Volunteer.DeleteAll();
            s_dal.Call.DeleteAll();
            s_dal.Assignment.DeleteAll();
        }
        /// <summary>
        /// Displays all records from the database (volunteers, calls, and assignments).
        /// </summary>
        private static void DisplayAllData()
        {
            foreach (var volunteer in s_dal.Volunteer.ReadAll())
            {
                Console.WriteLine(volunteer);
            }
            foreach (var call in s_dal.Call.ReadAll())
            {
                Console.WriteLine(call);
            }
            foreach (var assignment in s_dal.Assignment.ReadAll())
            {
                Console.WriteLine(assignment);
            }
        }
        /// <summary>
        /// Displays a CRUD menu for the specified entity (e.g., Volunteer, Assignment, Call) 
        /// and executes the corresponding CRUD operation based on user choice.
        /// Loops until the user selects the "Exit" option.
        /// </summary>
        /// <param name="entityName">The name of the entity (e.g., "Volunteer", "Assignment", etc.) for which CRUD operations are performed.</param>
        static void CrudMenu(string entityName)
        {
            // Displays the CRUD menu and prompts for user input.
            Console.WriteLine("Enter a number:");
            foreach (CrudChoice option in Enum.GetValues(typeof(CrudChoice)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }
            CrudChoice choice;
            Enum.TryParse(Console.ReadLine(), out choice);
            while (choice is not CrudChoice.Exit)
            {
                try
                {
                    // Switches to corresponding CRUD action based on user choice.
                    switch (choice)
                    {
                        case CrudChoice.Create:
                            CreateEntity(entityName);
                            break;
                        case CrudChoice.Read:
                            ReadEntityById(entityName);
                            break;
                        case CrudChoice.ReadAll:
                            ReadAllEntities(entityName);
                            break;
                        case CrudChoice.Update:
                            UpdateEntity(entityName);
                            break;
                        case CrudChoice.Delete:
                            DeleteEntity(entityName);
                            break;
                        case CrudChoice.DeleteAll:
                            DeleteAllEntities(entityName);
                            break;
                        default:
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
                // Prompts for input again.
                Console.WriteLine("Enter a number:");
                Enum.TryParse(Console.ReadLine(), out choice);
            }
        }
        /// <summary>
        /// Calls the specific creation method for each entity type.
        /// </summary>
        /// <param name="entityName">The name of the entity to create (e.g., "Volunteer", "Call", "Assignment").</param>
        static void CreateEntity(string entityName)
        {
            if (entityName == "Volunteer")
                CreateVolunteer();
            else if (entityName == "Call")
                CreateCall();
            else
                CreateAssignment();
        }
        /// <summary>
        /// Prompts the user to enter details for a new Volunteer and creates the Volunteer entity.
        /// </summary>
        static void CreateVolunteer()
        {
            Console.WriteLine("Enter Volunteer details:");
            Console.Write("ID: ");
            int id = int.Parse(Console.ReadLine()!);
            Console.Write("First Name: ");
            string? firstName = Console.ReadLine()!;
            Console.Write("Last Name: ");
            string? lastName = Console.ReadLine()!;
            Console.Write("Phone Number: ");
            string? phoneNumber = Console.ReadLine()!;
            Console.Write("Email: ");
            string? email = Console.ReadLine()!;
            Console.Write("IsActive? ");
            bool active = bool.Parse(Console.ReadLine()!);
            Console.WriteLine("Please enter Role: 'Manager' or 'Volunteer'.");
            Role role = (Role)Enum.Parse(typeof(Role), Console.ReadLine()!);
            Console.Write("Password: ");
            string? password = Console.ReadLine();
            Console.Write("Address: ");
            string? address = Console.ReadLine();
            Console.WriteLine("Enter location details:");
            Console.Write("Latitude: ");
            double latitude = double.Parse(Console.ReadLine()!);
            Console.Write("Longitude: ");
            double longitude = double.Parse(Console.ReadLine()!);
            Console.Write("Largest Distance: ");
            double largestDistance = double.Parse(Console.ReadLine()!);
            Console.Write("Distance Type (Air or Walk): ");
            DistanceType myDistanceType = (DistanceType)Enum.Parse(typeof(DistanceType), Console.ReadLine()!, true);
            // Creates the Volunteer using provided details.
            s_dal!.Volunteer.Create(new(id, firstName, lastName, phoneNumber, email, active, role, password, address, latitude, longitude, largestDistance, myDistanceType));
            Console.WriteLine("Volunteer created successfully!");
        }
        /// <summary>
        /// Prompts the user for details and creates a new Assignment entity with random volunteer and call IDs.
        /// </summary>
        static void CreateAssignment()
        {
            Random s_rand = new();
            Console.WriteLine("Enter Assignment details:");
            Console.Write("Entrance Time (yyyy-MM-dd HH:mm:ss): ");
            DateTime entranceTime = DateTime.Parse(Console.ReadLine()!);
            Console.Write("Exit Time (yyyy-MM-dd HH:mm:ss): ");
            DateTime exitTime = DateTime.Parse(Console.ReadLine()!);
            Console.Write("Finish Call Type (TakenCareOf, CanceledByVolunteer, CanceledByManager, Expired): ");
            FinishCallType finishCallType = (FinishCallType)Enum.Parse(typeof(FinishCallType), Console.ReadLine()!);
            List<Volunteer>? volunteers = s_dal!.Volunteer.ReadAll().ToList(); ;
            List<Call>? calls = s_dal!.Call.ReadAll().ToList(); ;
            int volunteerId = volunteers[s_rand.Next(volunteers.Count)].Id;
            int callId = calls[s_rand.Next(calls.Count)].Id;
            int id = s_dal!.Config.NextAssignmentId;
            s_dal.Assignment.Create(new Assignment(id, callId, volunteerId,  entranceTime, exitTime, finishCallType));
            Console.WriteLine("Assignment created successfully!");
        }
        /// <summary>
        /// Prompts the user for details and creates a new Call entity.
        /// </summary>
        static void CreateCall()
        {
            Console.WriteLine("Enter call details:");
            int id = s_dal!.Config.NextCallId;
            Console.Write("Call Type (MusicPerformance, MusicTherapy, SingingAndEmotionalSupport, GroupActivities, PersonalizedMusicCare): ");
            CallType myCallType = (CallType)Enum.Parse(typeof(CallType), Console.ReadLine()!, true);
            Console.Write("Address: ");
            string? address = Console.ReadLine()!;
            Console.Write("Latitude: ");
            double latitude = double.Parse(Console.ReadLine()!);
            Console.Write("Longitude: ");
            double longitude = double.Parse(Console.ReadLine()!);
            Console.Write("Open Time (yyyy-MM-dd HH:mm:ss): ");
            DateTime openTime = DateTime.Parse(Console.ReadLine()!);
            Console.Write("Max Finish Time (yyyy-MM-dd HH:mm:ss): ");
            DateTime maxFinishTime = DateTime.Parse(Console.ReadLine()!);
            Console.Write("Verbal Description: ");
            string? verbalDescription = Console.ReadLine()!;
            s_dal.Call.Create(new Call(id, myCallType, address, latitude, longitude, openTime, maxFinishTime, verbalDescription));
            Console.WriteLine("Call created successfully!");
        }
        /// <summary>
        /// Prompts the user for an ID and retrieves an entity by its ID (Volunteer, Assignment, or Call).
        /// </summary>
        /// <param name="entityName">The name of the entity to retrieve (e.g., "Volunteer", "Assignment", "Call").</param>
        static void ReadEntityById(string entityName)
        {
            Console.Write($"Enter {entityName} ID to read: ");
            int id = int.Parse(Console.ReadLine()!);
            dynamic entityId;
            if (entityName == "Volunteer")
                entityId = s_dal.Volunteer.Read(id)!;
            else if (entityName == "Assignment")
                entityId = s_dal.Assignment.Read(id)!;
            else
                entityId = s_dal.Call.Read(id)!;
            // Displays the entity or shows an error message if not found.
            if (entityId == null)
            { Console.WriteLine("No such ID found"); }
            else { Console.WriteLine(entityId); }
        }
        /// <summary>
        /// Reads and displays all entities of the specified type (Volunteer, Assignment, or Call).
        /// </summary>
        /// <param name="entityName">The name of the entity to read ("Volunteer", "Assignment", or "Call").</param>
        static void ReadAllEntities(string entityName)
        {
            if (entityName == "Volunteer")
            {
                foreach (var volunteer in s_dal.Volunteer.ReadAll())
                {
                    Console.WriteLine(volunteer);
                }
            }
            else if (entityName == "Assignment")
            {
                foreach (var assignment in s_dal.Assignment.ReadAll())
                {
                    Console.WriteLine(assignment);
                }
            }
            else
            {
                foreach (var call in s_dal.Call.ReadAll())
                {
                    Console.WriteLine(call);
                }
            }
        }
        /// <summary>
        /// Prompts the user to update an existing entity (Volunteer, Assignment, or Call) by ID.
        /// </summary>
        /// <param name="entityName">The name of the entity to update ("Volunteer", "Assignment", or "Call").</param>
        static void UpdateEntity(string entityName)
        {
            Console.Write($"Enter {entityName} ID to update: ");
            int id = int.Parse(Console.ReadLine()!);
            dynamic updateEntity;
            if (entityName == "Volunteer")
            {
                var existingEntity = s_dal.Volunteer?.Read(id);
                Console.WriteLine(existingEntity);
                updateEntity = UpdateVolunteer(existingEntity);
                s_dal.Volunteer?.Update(updateEntity);
            }
            else if (entityName == "Call")
            {
                var existingEntity = s_dal.Call?.Read(id);
                Console.WriteLine(existingEntity);
                updateEntity = UpdateCall(existingEntity);
                s_dal.Call?.Update(updateEntity);
            }
            else
            {
                var existingEntity = s_dal.Assignment?.Read(id);
                Console.WriteLine(existingEntity);
                updateEntity = UpdateAssignment(existingEntity);
                s_dal.Assignment?.Update(updateEntity);
            }
           
        }
        /// <summary>
        /// Prompts the user to update details of an existing Volunteer entity.
        /// </summary>
        /// <param name="existingVolunteer">The existing Volunteer entity to update.</param>
        /// <returns>A new Volunteer entity with updated details.</returns>
        static Volunteer UpdateVolunteer(Volunteer existingVolunteer)
        {
            Console.Write("First Name: ");
            string? firstName = Console.ReadLine();
            firstName = string.IsNullOrEmpty(firstName) ? existingVolunteer.FirstName : firstName;
            Console.Write("Last Name: ");
            string? lastName = Console.ReadLine();
            lastName = string.IsNullOrEmpty(lastName) ? existingVolunteer.LastName : lastName;
            Console.Write("Phone Number: ");
            string? phoneNumber = Console.ReadLine();
            phoneNumber = string.IsNullOrEmpty(phoneNumber) ? existingVolunteer.Phone : phoneNumber;
            Console.Write("Email: ");
            string? email = Console.ReadLine();
            email = string.IsNullOrEmpty(email) ? existingVolunteer.Email : email;
            Console.Write("IsActive? ");
            string active = Console.ReadLine()!;
            bool isActive = string.IsNullOrEmpty(active) ? existingVolunteer.Active : bool.Parse(active);
            Console.WriteLine("Please enter Role: 'Manager' or 'Volunteer'.");
            string roleInputString = Console.ReadLine()!;
            Role role = string.IsNullOrEmpty(roleInputString) ? existingVolunteer.MyRole : (Role)Enum.Parse(typeof(Role), roleInputString);
            Console.Write("Password: ");
            string? password = Console.ReadLine();
            password = string.IsNullOrEmpty(password) ? existingVolunteer.Password : password;
            Console.Write("Address: ");
            string? address = Console.ReadLine();
            address = string.IsNullOrEmpty(address) ? existingVolunteer.Address : address;
            Console.WriteLine("Enter location details:");
            Console.Write("Latitude: ");
            string latitudeInput = Console.ReadLine()!;
            double latitude = string.IsNullOrEmpty(latitudeInput) ? existingVolunteer.Latitude.GetValueOrDefault() : double.Parse(latitudeInput);
            Console.Write("Longitude: ");
            string longitudeInput = Console.ReadLine()!;
            double longitude = string.IsNullOrEmpty(longitudeInput) ? existingVolunteer.Longitude.GetValueOrDefault() : double.Parse(longitudeInput);
            Console.Write("Largest Distance: ");
            string largestDistance = Console.ReadLine()!;
            double maxDistance = string.IsNullOrEmpty(largestDistance) ? existingVolunteer.LargestDistance.GetValueOrDefault() : double.Parse(largestDistance);
            Console.Write("Distance Type (Air or Walk): ");
            string myDistanceType = Console.ReadLine()!;
            DistanceType distanceType = string.IsNullOrEmpty(myDistanceType) ? existingVolunteer.MyDistanceType : (DistanceType)Enum.Parse(typeof(DistanceType), myDistanceType);
            return new Volunteer(existingVolunteer.Id, firstName, lastName, phoneNumber, email, isActive, role, password, address, latitude, longitude, maxDistance, distanceType);
        }
        /// <summary>
        /// Prompts the user to update details of an existing Call entity.
        /// </summary>
        /// <param name="existingCall">The existing Call entity to update.</param>
        /// <returns>A new Call entity with updated details.</returns>
        static Call UpdateCall(Call existingCall)
        {
            Console.Write("Call Type (MusicPerformance, MusicTherapy, SingingAndEmotionalSupport, GroupActivities, PersonalizedMusicCare): ");
            string myCallTypeInput = Console.ReadLine()!;
            CallType myCallType = string.IsNullOrEmpty(myCallTypeInput) ? existingCall.MyCallType : (CallType)Enum.Parse(typeof(CallType), myCallTypeInput);
            Console.Write("Address: ");
            string address = string.IsNullOrEmpty(Console.ReadLine()) ? existingCall.Address : Console.ReadLine()!;
            Console.Write("Latitude: ");
            string latitudeInput = Console.ReadLine()!;
            double latitude = string.IsNullOrEmpty(latitudeInput) ? existingCall.Latitude : double.Parse(latitudeInput);
            Console.Write("Longitude: ");
            string longitudeInput = Console.ReadLine()!;
            double longitude = string.IsNullOrEmpty(longitudeInput) ? existingCall.Longitude : double.Parse(longitudeInput);
            Console.Write("Open Time (yyyy-MM-dd HH:mm:ss): ");
            string openingTimeInput = Console.ReadLine()!;
            DateTime openingTime = string.IsNullOrEmpty(openingTimeInput) || !DateTime.TryParse(openingTimeInput, out DateTime ot) ? existingCall.OpenTime : ot;
            Console.Write("Max Finish Time (yyyy-MM-dd HH:mm:ss): ");
            string maxTimeInput = Console.ReadLine()!;
            DateTime? maxTime = string.IsNullOrEmpty(maxTimeInput) || !DateTime.TryParse(maxTimeInput, out DateTime mt) ? existingCall.MaxFinishTime : mt; Console.Write("Verbal Description: ");
            Console.Write("Verbal Description: ");
            string? description = string.IsNullOrEmpty(Console.ReadLine()) ? existingCall.VerbalDescription : Console.ReadLine();
            int id = existingCall.Id;
            return new Call(id, myCallType, address, latitude, longitude, openingTime, maxTime, description);
            
        }
        /// <summary>
        /// Prompts the user to update an existing Assignment entity's details.
        /// </summary>
        /// <param name="existingAssignment">The existing Assignment entity to update.</param>
        /// <returns>A new Assignment entity with updated details.</returns>
        static Assignment UpdateAssignment(Assignment existingAssignment)
        {
            int Id = existingAssignment.Id;
            int CallId = existingAssignment.CallId;
            int VolunteerId = existingAssignment.VolunteerId;
            Console.Write("Entrance Time (yyyy-MM-dd HH:mm:ss): ");
            string entryTimeStr = Console.ReadLine()!;
            DateTime entryTime = string.IsNullOrEmpty(entryTimeStr) || !DateTime.TryParse(entryTimeStr, out DateTime ent) ? existingAssignment.EntranceTime : ent;
            Console.Write("Exit Time (yyyy-MM-dd HH:mm:ss): ");
            string endingTimeStr = Console.ReadLine()!;
            DateTime? endingTime = string.IsNullOrEmpty(endingTimeStr) || !DateTime.TryParse(endingTimeStr, out DateTime end) ? existingAssignment.ExitTime : end;
            Console.Write("Finish Call Type (TakenCareOf, CanceledByVolunteer, CanceledByManager, Expired): ");
            string endingTimeTypeInput = Console.ReadLine()!;
            FinishCallType? endingTimeType = string.IsNullOrEmpty(endingTimeTypeInput) ? existingAssignment.FinishCallType : (FinishCallType?)int.Parse(endingTimeTypeInput);
            return new Assignment(Id, CallId, VolunteerId, entryTime, endingTime, endingTimeType);
            
        }
        /// <summary>
        /// Deletes an entity (Volunteer, Call, or Assignment) by ID.
        /// </summary>
        /// <param name="entityName">The name of the entity to delete ("Volunteer", "Call", or "Assignment").</param>
        static void DeleteEntity(string entityName)
        {
            Console.Write($"Enter {entityName} ID to delete: ");
            int id = int.Parse(Console.ReadLine()!);
            if (entityName == "Volunteer")
                s_dal.Volunteer!.Delete(id);
            else if(entityName=="Call")
                s_dal.Call!.Delete(id);
            else
                s_dal.Assignment!.Delete(id);
            Console.WriteLine($"{entityName} deleted successfully!");
        }
        /// <summary>
        /// Deletes all entities of the specified type (Volunteer, Call, or Assignment).
        /// </summary>
        /// <param name="entityName">The name of the entity to delete ("Volunteer", "Call", or "Assignment").</param>
        static void DeleteAllEntities(string entityName)
        {
            Console.WriteLine($"Deleting all entities of type: {entityName}");
            s_dal.Volunteer.DeleteAll(); 
            s_dal.Call.DeleteAll();
            s_dal.Assignment.DeleteAll();
        }
        /// <summary>
        /// Displays the configuration menu, allowing the user to perform various clock-related actions.
        /// </summary>
        static void ShowConfigMenu()
        {
            Console.Write("Choose an action: ");
            // Display available configuration choices.
            foreach (ConfigChoice option in Enum.GetValues(typeof(ConfigChoice)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }
            // Read user input for menu selection.
            ConfigChoice choice;
            Enum.TryParse(Console.ReadLine(), out choice);
            // Read user input for menu selection.
            while (choice is not ConfigChoice.Exit)
            {
                try
                {
                    // Handle each choice case.
                    switch (choice)
                    {
                        case ConfigChoice.Exit:
                            return;
                        case ConfigChoice.AdvanceClockMinute:
                            s_dal!.Config.Clock = s_dal.Config.Clock.AddMinutes(1);
                            Console.WriteLine("System clock advanced by 1 minute.");
                            break;
                        case ConfigChoice.AdvanceClockHour:
                            s_dal!.Config.Clock = s_dal.Config.Clock.AddHours(1);
                            Console.WriteLine("System clock advanced by 1 hour.");
                            break;
                        case ConfigChoice.AdvanceClockByDay:
                            s_dal!.Config.Clock = s_dal.Config.Clock.AddDays(1);
                            break;
                        case ConfigChoice.AdvanceClockByMonth:
                            s_dal!.Config.Clock = s_dal.Config.Clock.AddMonths(1);
                            break;
                        case ConfigChoice.AdvanceClockByYear:
                            s_dal!.Config.Clock = s_dal.Config.Clock.AddYears(1);
                            break;
                        case ConfigChoice.ShowCurrentClock:
                            Console.WriteLine($"Current system clock value: {s_dal!.Config.Clock}");
                            break;
                        case ConfigChoice.ChangeClock:
                            Console.Write("Enter a new value for the system clock in format YY MM DD HH MM SS: ");
                            string times = Console.ReadLine()!;
                            string[] timesArray = times.Split(' ');
                            try
                            {
                                int year = int.Parse(timesArray[0]);
                                int month = int.Parse(timesArray[1]);
                                int day = int.Parse(timesArray[2]);
                                int hour = int.Parse(timesArray[3]);
                                int minute = int.Parse(timesArray[4]);
                                int second = int.Parse(timesArray[5]);
                                s_dal.Config.Clock = new DateTime(year, month, day, hour, minute, second);
                            }
                            catch (FormatException ex)
                            {
                                Console.WriteLine("Error: Invalid date format. Please ensure you enter the values correctly (YY MM DD HH MM SS).");
                            }
                            catch (ArgumentOutOfRangeException ex)
                            {
                                Console.WriteLine("Error: One or more values are out of range. Please check the values you entered.");
                            }
                            break;
                        case ConfigChoice.ResetConfig:
                            s_dal.Config.Reset();
                            Console.WriteLine("Config values reset to default.");
                            break;

                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                }
                // Prompt for the next action.
                Console.WriteLine("Enter a number:");
                Enum.TryParse(Console.ReadLine(), out choice);
            }
        }
    }
}