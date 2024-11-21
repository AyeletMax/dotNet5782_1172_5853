using Dal;
using DalApi;
using DO;

namespace DalTest
{
    enum MainMenuChoice
    {
        Exit, Volunteer, Assignments, Calls, Config
    }
    //, InitializeData, ResetData
    enum CrudChoice
    {
        Exit, Create, Read, ReadAll, Update, Delete, DeleteAll
    }
    enum ConfigChoice
    {
        Exit, AdvanceClockMinute, AdvanceClockHour, ShowCurrentClock, SetConfigValue, ResetConfig
    }
    internal class Program
    {
        private static IAssignment? s_dalAssignment = new AssignmentImplementation();
        private static ICall? s_dalCall = new CallImplementation();
        private static IConfig? s_dalConfi = new ConfigImplementation();
        private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
        static void Main(string[] args)
        {
            try
            {
                //        bool running = true;
                //        while (running)
                //        {
                //            mainMenu();
                //            int mainChoice = int.Parse(Console.ReadLine());

                //            MainMenuChoice mainMenuChoice = (MainMenuChoice)mainChoice;

                //            switch (mainMenuChoice)
                //            {
                //                case MainMenuChoice.Exit:
                //                    running = false;
                //                    break;

                //                case MainMenuChoice.Volunteer:
                //                    crudMenu("Volunteer", s_dalVolunteer);
                //                    break;

                //                case MainMenuChoice.Assignments:
                //                    crudMenu("Assignment", s_dalAssignment);
                //                    break;

                //                case MainMenuChoice.Calls:
                //                    crudMenu("Call", s_dalCall);
                //                    break;

                //                case MainMenuChoice.Config:
                //                    configMenu();
                //                    break;

                //                default:
                //                    Console.WriteLine("Invalid choice. Please try again.");
                //                    break;
                //            }
                //
                //  }
                mainMenu();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            static void mainMenu()
            {
                bool running = true;
                while (running)
                {
                    Console.WriteLine("Main Menu");
                    Console.WriteLine("1. Volunteer CRUD Menu");
                    Console.WriteLine("2. Assignment CRUD Menu");
                    Console.WriteLine("3. Call CRUD Menu");
                    Console.WriteLine("3. Config CRUD Menu");
                    Console.WriteLine("0. Exit");
                    int mainChoice = int.Parse(Console.ReadLine());

                     MainMenuChoice mainMenuChoice = (MainMenuChoice)mainChoice;
                    switch (mainMenuChoice) { 
                        case MainMenuChoice.Exit:
                            running = false;
                            break;
                        case MainMenuChoice.Volunteer:
                            VolunteerCrudMenu();
                            break;
                        case MainMenuChoice.Assignments:
                            AssignmentCrudeMenu();
                            break; 
                        case MainMenuChoice.Calls:
                            CallCrudeMenu();
                            break;
                        case MainMenuChoice.Config:
                            ConfigCrudeMenu();
                            break;

                    }
                }
            }
            static void VolunteerCrudMenu()
            {
                bool running = true;
                while (running)
                {
                    Console.Clear();
                    Console.WriteLine("=== Volunteer CRUD Menu ===");

                    Console.WriteLine("1. Create Volunteer");
                    Console.WriteLine("2. Read Volunteer by ID");
                    Console.WriteLine("3. Read All Volunteers");
                    Console.WriteLine("4. Update Volunteer");
                    Console.WriteLine("5. Delete Volunteer");
                    Console.WriteLine("6. Delete All Volunteers");
                    Console.WriteLine("0. Back to Main Menu");
                    Console.Write("Select an option: ");

                    int option = int.Parse(Console.ReadLine());

                    switch (option)
                    {
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

                        case 0:
                            running = false;
                            break;

                        default:
                            Console.WriteLine("Invalid option, please try again.");
                            break;
                    }
                }
            }

            static void CreateVolunteer()
            {
                Console.WriteLine("Enter Volunteer details:");
                Console.Write("ID: ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("First Name: ");
                string firstName = Console.ReadLine();
                Console.Write("Last Name: ");
                string lastName = Console.ReadLine();
                Console.Write("Phone Number: ");
                string phoneNumber = Console.ReadLine();
                Console.Write("Email: ");
                string email = Console.ReadLine();
                Console.Write("Password: ");
                string password = Console.ReadLine();
                Console.Write("Address: ");
                string address = Console.ReadLine();
                bool active = true;//איך שולחים את הACTIVE?
                s_dalVolunteer.Create(new Volunteer(id, firstName, lastName, phoneNumber, email, active, password, address));//מה צריך לשלוח בדיוק? הפרויקט הכי לא מובן במדינה!!!
                Console.WriteLine("Volunteer created successfully!");
            }

            static void ReadVolunteerById()
            {
                Console.Write("Enter Volunteer ID to read: ");
                int id = int.Parse(Console.ReadLine());

                var volunteer = s_dalVolunteer.Read(id);
                if (volunteer != null)
                {
                    Console.WriteLine($"Volunteer Found: {volunteer}");
                }
                else
                {
                    Console.WriteLine("Volunteer not found.");
                }
            }

            static void ReadAllVolunteers()
            {
                var volunteers = s_dalVolunteer!.ReadAll();
                Console.WriteLine("All Volunteers:");
                foreach (var volunteer in volunteers)
                {
                    Console.WriteLine(volunteer);
                }
            }

            static void UpdateVolunteer()//מה לעדכן??????? הפרויקט הכי מחריד בתבל!!פיכסס
            {
                Console.Write("Enter Volunteer ID to update: ");
                int id = int.Parse(Console.ReadLine());

                var volunteer = s_dalVolunteer!.Read(id);
                if (volunteer != null)
                {
                    Console.WriteLine($"Current Volunteer: {volunteer}");
                    Console.Write("Enter new name: ");
                    string newName = Console.ReadLine();
                    var updatedVolunteer = volunteer with { FirstName = newName };

                    s_dalVolunteer.Update(updatedVolunteer);
                    Console.WriteLine("Volunteer updated successfully!");
                }
                else
                {
                    Console.WriteLine("Volunteer not found.");
                }
            }

            static void DeleteVolunteer()
            {
                Console.Write("Enter Volunteer ID to delete: ");
                int id = int.Parse(Console.ReadLine());

                s_dalVolunteer!.Delete(id);
                Console.WriteLine("Volunteer deleted successfully!");
            }

            static void DeleteAllVolunteers()
            {
                s_dalVolunteer!.DeleteAll();
                Console.WriteLine("All Volunteers deleted successfully!");
            }
            //}
            //static void mainMenu()
            //{
            //    Console.Clear();
            //    Console.WriteLine("Main Menu");
            //    for (int i = 0; i < Enum.GetValues(typeof(MainMenuChoice)).Length; i++)
            //    {
            //        Console.WriteLine($"{(int)i}. {i}");
            //    }
            //    Console.Write("Select an option: ");
            //}

            //static void crudMenu(string entityName, dynamic dal)
            //{
            //    bool running = true;
            //    while (running)
            //    {
            //        Console.WriteLine($" {entityName} CRUD Menu ");

            //        for (int i = 0; i < Enum.GetValues(typeof(CrudChoice)).Length; i++)
            //        {
            //            Console.WriteLine($"{(int)i}. {i}");
            //        }
            //        Console.Write("Select an option: ");

            //        int option = int.Parse(Console.ReadLine());

            //        CrudChoice crudChoice = (CrudChoice)option;
            //        switch (crudChoice)
            //        {
            //            case CrudChoice.Exit:
            //                running = false;
            //                break;
            //            case CrudChoice.Create:
            //                CreateEntity(entityName, dal);
            //                break;

            //            case CrudChoice.Read:
            //                ReadEntityById(entityName, dal);
            //                break;

            //            case CrudChoice.ReadAll:
            //                ReadAllEntities(entityName, dal);
            //                break;

            //            case CrudChoice.Update:
            //                UpdateEntity(entityName, dal);
            //                break;

            //            case CrudChoice.Delete:
            //                DeleteEntity(entityName, dal);
            //                break;

            //            case CrudChoice.DeleteAll:
            //                DeleteAllEntities(entityName, dal);
            //                break;

            //            default:
            //                Console.WriteLine("Invalid option, please try again.");
            //                break;
            //        }
            //    }
            //}
            //static void configMenu()
            //{
            //    bool running = true;
            //    while (running)
            //    {
            //        Console.WriteLine("Config Menu");

            //        for (int i = 0; i < Enum.GetValues(typeof(ConfigChoice)).Length; i++)
            //        {
            //            Console.WriteLine($"{(int)i}. {i}");
            //        }
            //        Console.Write("Select an option: ");
            //        int option = int.Parse(Console.ReadLine());
            //        ConfigChoice configChoice = (ConfigChoice)option;
            //        switch (configChoice)
            //        {
            //            case ConfigChoice.Exit:
            //                running = false;
            //                break;

            //            case ConfigChoice.AdvanceClockMinute:
            //                AdjustClock();
            //                break;

            //            case ConfigChoice.AdvanceClockHour:
            //                ShowCurrentClock();
            //                break;
            //            case ConfigChoice.ShowCurrentClock:
            //                ShowCurrentClock();
            //                break;

            //            case ConfigChoice.SetConfigValue:
            //                ShowCurrentClock();
            //                break;
            //            case ConfigChoice.ResetConfig:
            //                ResetConfig();
            //                break;

            //            default:
            //                Console.WriteLine("Invalid option, please try again.");
            //                break;
            //        }
            //    }
            //}
        }
    }
}