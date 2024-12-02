using Dal;
using DalApi;
using DO;

namespace DalTest
{
    internal class Program
    {
        //private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        //private static ICall? s_dalCall = new CallImplementation();
        //private static IConfig? s_dalConfig = new ConfigImplementation();
        //private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); 
        static readonly IDal s_dal = new Dal.DalList(); //stage 2
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

        static void mainMenu()
        {
            Console.WriteLine("Enter a number:");
            foreach (MainMenuChoice choice in Enum.GetValues(typeof(MainMenuChoice)))
            {
                Console.WriteLine($"{(int)choice}. {choice}");
            }
            MainMenuChoice mainMenuChoice;
            Enum.TryParse(Console.ReadLine(), out mainMenuChoice);
            while (mainMenuChoice is not MainMenuChoice.Exit)
            {
                try
                {
                    switch (mainMenuChoice)
                    {
                        case MainMenuChoice.Volunteer:
                            try
                            {
                                CrudMenu("Volunteer", s_dal.Volunteer);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in Volunteer menu: {ex.Message}");
                            }
                            break;

                        case MainMenuChoice.Assignments:
                            try
                            {
                                CrudMenu("Assignment", s_dal.Assignment);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in Assignments menu: {ex.Message}");
                            }
                            break;

                        case MainMenuChoice.Calls:
                            try
                            {
                                CrudMenu("Call", s_dal.Call);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in Calls menu: {ex.Message}");
                            }
                            break;

                        case MainMenuChoice.Config:
                            try
                            {
                                ShowConfigMenu();
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error in Config menu: {ex.Message}");
                            }
                            break;

                        case MainMenuChoice.InitializeData:
                            try
                            {
                                Initialization.DO(s_dal);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error initializing data: {ex.Message}");
                            }
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
                Console.WriteLine("Enter a number:");
                Enum.TryParse(Console.ReadLine(), out mainMenuChoice);

            }
        }
        private static void ResetDatabase()
        {
            s_dal.Config.Reset();
            s_dal.Volunteer.DeleteAll();
            s_dal.Call.DeleteAll();
            s_dal.Assignment.DeleteAll();
        }
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

        static void CrudMenu(string entityName, dynamic dal)
        {
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
                    switch (choice)
                    {
                        case CrudChoice.Create:
                            try
                            {
                                CreateEntity(entityName, dal);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while creating entity: {ex.Message}");
                            }
                            break;
                        case CrudChoice.Read:
                            try
                            {
                                ReadEntityById(entityName, dal);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while reading entity: {ex.Message}");
                            }
                            break;
                        case CrudChoice.ReadAll:
                            try
                            {
                                ReadAllEntities(entityName, dal);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while reading all entities: {ex.Message}");
                            }
                            break;
                        case CrudChoice.Update:
                            try
                            {
                                UpdateEntity(entityName, dal);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while updating entity: {ex.Message}");
                            }
                            break;
                        case CrudChoice.Delete:
                            try
                            {
                                DeleteEntity(entityName, dal);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while deleting entity: {ex.Message}");
                            }
                            break;
                        case CrudChoice.DeleteAll:
                            try
                            {
                                DeleteAllEntities(entityName, dal);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error while deleting all entities: {ex.Message}");
                            }
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
                Console.WriteLine("Enter a number:");
                Enum.TryParse(Console.ReadLine(), out choice);
            }
        }
        static void CreateEntity(string entityName, dynamic dal)
        {
            if (entityName == "Volunteer")
                CreateVolunteer();
            else if (entityName == "Call")
                CreateCall();
            else
                CreateAssignment();
        }
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
            s_dal!.Volunteer.Create(new(id, firstName, lastName, phoneNumber, email, active, role, password, address, latitude, longitude, largestDistance, myDistanceType));
            Console.WriteLine("Volunteer created successfully!");
        }
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
            List<Volunteer>? volunteers = s_dal!.Volunteer.ReadAll();
            List<Call>? calls = s_dal!.Call.ReadAll();
            int volunteerId = volunteers[s_rand.Next(volunteers.Count)].Id;
            int callId = calls[s_rand.Next(calls.Count)].Id;
            int id = s_dal!.Config.NextAssignmentId;
            s_dal.Assignment.Create(new Assignment(id, callId, volunteerId,  entranceTime, exitTime, finishCallType));
            Console.WriteLine("Assignment created successfully!");
        }
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

        static void ReadEntityById(string entityName, dynamic dal)
        {
            Console.Write($"Enter {entityName} ID to read: ");
            int id = int.Parse(Console.ReadLine()!);
            var entityId = dal.Read(id);
            if (entityId == null)
            { Console.WriteLine("No such ID found"); }
            else { Console.WriteLine(entityId); }
        }

        static void ReadAllEntities(string entityName, dynamic dal)
        {
            var dals = dal!.ReadAll();
            Console.WriteLine($"All {entityName}s:");
            foreach (var entity in dals)
            {
                Console.WriteLine(entity);
            }
        }
        static void UpdateEntity(string entityName, dynamic dal)
        {
            Console.Write($"Enter {entityName} ID to update: ");
            int id = int.Parse(Console.ReadLine()!);
            var existingEntity = dal?.Read(id);
            Console.WriteLine(existingEntity);
            dynamic updateEntity;
            if (entityName == "Volunteer")
            {
                updateEntity = UpdateVolunteer(existingEntity);
            }
            else if (entityName == "Call")
            {
                updateEntity = UpdateCall(existingEntity);
            }
            else
            {
                updateEntity = UpdateAssignment(existingEntity);
            }
            dal?.Update(updateEntity);
        }
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
        static void DeleteEntity(string entityName, dynamic dal)
        {
            Console.Write($"Enter {entityName} ID to delete: ");
            int id = int.Parse(Console.ReadLine()!);
            dal!.Delete(id);
            Console.WriteLine($"{entityName} deleted successfully!");
        }

        static void DeleteAllEntities(string entityName, dynamic dal)
        {
            Console.WriteLine($"Deleting all entities of type: {entityName}");
            dal.DeleteAll();
        }
        static void ShowConfigMenu()
        {
            Console.Write("Choose an action: ");

            foreach (ConfigChoice option in Enum.GetValues(typeof(ConfigChoice)))
            {
                Console.WriteLine($"{(int)option}. {option}");
            }
            ConfigChoice choice;
            Enum.TryParse(Console.ReadLine(), out choice);
            while (choice is not ConfigChoice.Exit)
            {
                try
                {
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
                Console.WriteLine("Enter a number:");
                Enum.TryParse(Console.ReadLine(), out choice);
            }
        }
    }
}