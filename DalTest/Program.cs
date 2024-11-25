using Dal;
using DalApi;
using DO;

namespace DalTest
{
    internal class Program
    {
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        private static ICall? s_dalCall = new CallImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
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
                switch (mainMenuChoice)
                {
                    case MainMenuChoice.Volunteer:
                        CrudMenu("Volunteer", s_dalVolunteer);
                        break;
                    case MainMenuChoice.Assignments:
                        CrudMenu("Assignment", s_dalAssignment);
                        break;
                    case MainMenuChoice.Calls:
                        CrudMenu("Call", s_dalCall);
                        break;
                    case MainMenuChoice.Config:
                        ShowConfigMenu();
                        break;
                    case MainMenuChoice.InitializeData:
                        Initialization.DO(s_dalAssignment, s_dalCall, s_dalConfig, s_dalVolunteer);
                        break;
                    case MainMenuChoice.DisplayAllData:
                        DisplayAllData();
                        break;
                    case MainMenuChoice.ResetDatabase:
                        ResetDatabase();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
                Console.WriteLine("Enter a number:");
                Enum.TryParse(Console.ReadLine(), out mainMenuChoice);

            }
        }
        private static void ResetDatabase()
        {
            s_dalConfig!.Reset();
            s_dalVolunteer!.DeleteAll();
            s_dalCall!.DeleteAll();
            s_dalAssignment!.DeleteAll();
        }
        private static void DisplayAllData()
        {
            foreach (var volunteer in s_dalVolunteer!.ReadAll())
            {
                Console.WriteLine(volunteer);
            }
            Console.WriteLine(s_dalCall!.ReadAll());
            Console.WriteLine(s_dalAssignment!.ReadAll());
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
                switch (choice)
                {
                    case CrudChoice.Create:
                        CreateEntity(entityName, dal);
                        break;
                    case CrudChoice.Read:
                        ReadEntityById(entityName, dal);
                        break;
                    case CrudChoice.ReadAll:
                        ReadAllEntities(entityName, dal);
                        break;
                    case CrudChoice.Update:
                        UpdateEntity(entityName, dal);
                        break;
                    case CrudChoice.Delete:
                        DeleteEntity(entityName, dal);
                        break;
                    case CrudChoice.DeleteAll:
                        DeleteAllEntities(entityName, dal);
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
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
            int id = int.Parse(Console.ReadLine());
            Console.Write("First Name: ");
            string? firstName = Console.ReadLine();
            Console.Write("Last Name: ");
            string? lastName = Console.ReadLine();
            Console.Write("Phone Number: ");
            string? phoneNumber = Console.ReadLine();
            Console.Write("Email: ");
            string? email = Console.ReadLine();
            Console.Write("IsActive? ");
            bool active = bool.Parse(Console.ReadLine());
            Console.WriteLine("Invalid role. Please enter 'Manager' or 'Volunteer'.");
            Role role = (Role)Enum.Parse(typeof(Role), Console.ReadLine());
            Console.Write("Password: ");
            string? password = Console.ReadLine();
            Console.Write("Address: ");
            string? address = Console.ReadLine();
            Console.WriteLine("Enter location details:");
            Console.Write("Latitude: ");
            double latitude = double.Parse(Console.ReadLine());
            Console.Write("Longitude: ");
            double longitude = double.Parse(Console.ReadLine());
            Console.Write("Largest Distance: ");
            double largestDistance = double.Parse(Console.ReadLine());
            Console.Write("Distance Type (Air or Land): ");
            DistanceType myDistanceType = (DistanceType)Enum.Parse(typeof(DistanceType), Console.ReadLine(), true);
            s_dalVolunteer!.Create(new(id, firstName, lastName, phoneNumber, email, active, role, password, address, latitude, longitude, largestDistance, myDistanceType));
            Console.WriteLine("Volunteer created successfully!");
        }
        static void CreateAssignment()
        {
            Console.WriteLine("Enter Assignment details:");
            Console.Write("Entrance Time (yyyy-MM-dd HH:mm:ss): ");
            DateTime entranceTime = DateTime.Parse(Console.ReadLine());
            Console.Write("Exit Time (yyyy-MM-dd HH:mm:ss): ");
            DateTime exitTime = DateTime.Parse(Console.ReadLine());
            Console.Write("Finish Call Type (TakenCareOf, CanceledByVolunteer, CanceledByManager, Expired): ");
            FinishCallType finishCallType = (FinishCallType)Enum.Parse(typeof(FinishCallType), Console.ReadLine(), true);
            s_dalAssignment!.Create(new Assignment(entranceTime, exitTime, finishCallType));
            Console.WriteLine("Assignment created successfully!");
        }
        static void CreateCall()
        {
            Console.WriteLine("Enter call details:");
            Console.Write("Call Type (MusicPerformance, MusicTherapy, SingingAndEmotionalSupport, GroupActivities, PersonalizedMusicCare): ");
            CallType myCallType = (CallType)Enum.Parse(typeof(CallType), Console.ReadLine(), true);
            Console.Write("Address: ");
            string? address = Console.ReadLine();
            Console.Write("Latitude: ");
            double latitude = double.Parse(Console.ReadLine());
            Console.Write("Longitude: ");
            double longitude = double.Parse(Console.ReadLine());
            Console.Write("Open Time (yyyy-MM-dd HH:mm:ss): ");
            DateTime openTime = DateTime.Parse(Console.ReadLine());
            Console.Write("Max Finish Time (yyyy-MM-dd HH:mm:ss): ");
            DateTime maxFinishTime = DateTime.Parse(Console.ReadLine());
            Console.Write("Verbal Description: ");
            string? verbalDescription = Console.ReadLine();
            s_dalCall!.Create(new Call(myCallType, address, latitude, longitude, openTime, maxFinishTime, verbalDescription));
            Console.WriteLine("Call created successfully!");
        }

        static void ReadEntityById(string entityName, dynamic dal)
        {
            Console.Write($"Enter {entityName} ID to read: ");
            int id = int.Parse(Console.ReadLine());
            var entityId = dal.Read(id);
        }

        static void ReadAllEntities(string entityName, dynamic dal)
        {
            var dals = dal!.ReadAll();
            Console.WriteLine($"All {entityName}s:");
            foreach (var entity in dals)
            {
                Console.WriteLine(dals);
            }
        }
        //מה בדיוק צריך לעדכן?
        static void UpdateEntity(string entityName, dynamic dal)
        {
            Console.Write($"Enter {entityName} ID to update: ");
            //int id = int.Parse(Console.ReadLine());

            //var entity = dal!.Read(id);
            //if (entity != null)
            //{
            //    Console.WriteLine($"Current {entityName}: {entity}");
            //    Console.Write("Enter new ID: ");
            //    Console.Write("Enter new name: ");
            //    string newName = Console.ReadLine();
            //    var updatedVolunteer = entity with { FirstName = newName };

            //    Console.WriteLine($"{entityName} updated successfully!");
            //}
            //else
            //{
            //    Console.WriteLine($"{entityName} not found.");
            //}
        }
        static void DeleteEntity(string entityName, dynamic dal)
        {
            Console.Write($"Enter {entityName} ID to delete: ");
            int id = int.Parse(Console.ReadLine());
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
                switch (choice)
                {
                    case ConfigChoice.Exit:
                        return;
                    case ConfigChoice.AdvanceClockMinute:
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddMinutes(1);
                        Console.WriteLine("System clock advanced by 1 minute.");
                        break;
                    case ConfigChoice.AdvanceClockHour:
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddHours(1);
                        Console.WriteLine("System clock advanced by 1 hour.");
                        break;
                    case ConfigChoice.AdvanceClockByDay:
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddDays(1);
                        break;
                    case ConfigChoice.AdvanceClockByMonth:
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddMonths(1);
                        break;
                    case ConfigChoice.AdvanceClockByYear:
                        s_dalConfig!.Clock = s_dalConfig.Clock.AddYears(1);
                        break;
                    case ConfigChoice.ShowCurrentClock:
                        Console.WriteLine($"Current system clock value: {s_dalConfig!.Clock}");
                        break;
                    case ConfigChoice.ChangeClock:
                        Console.Write("Enter a new value for the system clock : ");
                        string times = Console.ReadLine()!;
                        string[] timesArray = times.Split(',');
                        int year = int.Parse(timesArray[0]);
                        int month = int.Parse(timesArray[1]);
                        int day = int.Parse(timesArray[2]);
                        int hour = int.Parse(timesArray[3]);
                        int minute = int.Parse(timesArray[4]);
                        int second = int.Parse(timesArray[5]);
                        s_dalConfig!.Clock = new DateTime(year, month, day, hour, minute, second);
                        break;
                    case ConfigChoice.ResetConfig:
                        s_dalConfig!.Reset();
                        Console.WriteLine("Config values reset to default.");
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
                Console.WriteLine("Enter a number:");
                Enum.TryParse(Console.ReadLine(), out choice);
                //Console.WriteLine("\nPress Enter to continue...");
                //Console.ReadLine();
            }
        }
    }
}


    



