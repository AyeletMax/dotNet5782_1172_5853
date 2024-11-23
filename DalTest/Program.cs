using Dal;
using DalApi;
using DO;

namespace DalTest;

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
//Initialization.Do(s_dalAssignment, s_dalCall, s_dalConfig, s_dalVolunteer);
  
    private static IAssignment? s_dalAssignment = new AssignmentImplementation();
    private static ICall? s_dalCall = new CallImplementation();
    private static IConfig? s_dalConfig = new ConfigImplementation();
    private static IVolunteer? s_dalVolunteer = new VolunteerImplementation();
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
                switch (mainMenuChoice)
                {
                    case MainMenuChoice.Exit:
                        running = false;
                        break;
                    case MainMenuChoice.Volunteer:
                        crudMenu("Volunteer", s_dalVolunteer);
                        break;
                    case MainMenuChoice.Assignments:
                        crudMenu("Assignment", s_dalAssignment);
                        break;
                    case MainMenuChoice.Calls:
                        crudMenu("Call", s_dalCall);
                        break;
                    case MainMenuChoice.Config:
                        configMenu();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;

                }
            }
        }
        static void crudMenu(string entityName, dynamic dal)
        {
            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine($"{entityName} CRUD Menu");
                Console.WriteLine("1. Create");
                Console.WriteLine("2. Read by ID");
                Console.WriteLine("3. Read All");
                Console.WriteLine("4. Update");
                Console.WriteLine("5. Delete");
                Console.WriteLine("6. Delete All");
                Console.WriteLine("0. Back to Main Menu");

                int option = int.Parse(Console.ReadLine());
                CrudChoice crudChoice = (CrudChoice)option;

                switch (crudChoice)
                {
                    case CrudChoice.Exit:
                        running = false;
                        break;
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

        static void ReadEntityById(string entityName, dynamic dal)
        {
            Console.Write($"Enter {entityName} ID to read: ");
            int id = int.Parse(Console.ReadLine());

            var entityId = dal.Read(id);
            if (entityId != null)
            {
                Console.WriteLine($"{entityName} Found: {entityId}");
            }
            else
            {
                Console.WriteLine($"{entityName} not found.");
            }
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
            int id = int.Parse(Console.ReadLine());

            var entity = dal!.Read(id);
            if (entity != null)
            {
                Console.WriteLine($"Current {entityName}: {entity}");
                Console.Write("Enter new name: ");
                string newName = Console.ReadLine();
                var updateEntity = entity with { FirstName = newName };

                dal.Update(updateEntity);
                Console.WriteLine($"{entityName} updated successfully!");
            }
            else
            {
                Console.WriteLine($"{entityName} not found.");
            }
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
    static void CreateAssignment()
    {
        Console.WriteLine("Enter Assignment details:");
        Console.Write("ID: ");
        int Id = int.Parse(Console.ReadLine());
        s_dalAssignment.Create(new Assignment(Id));//מה בדיוק שולחים ועבור אילו שדות יוצרים?
        Console.WriteLine("Assignment created successfully!");
    }
    static void CreateCall()
    {
        Console.WriteLine("Enter Assignment details:");
        Console.Write("ID: ");
        int Id = int.Parse(Console.ReadLine());
        s_dalCall.Create(new Call(Id));//מה בדיוק שולחים ועבור אילו שדות יוצרים?
        Console.WriteLine("Assignment created successfully!");

    }

}

//        static void VolunteerCrudMenu()
//        {
//            bool running = true;
//            while (running)
//            {
//                Console.Clear();
//                Console.WriteLine("=== Volunteer CRUD Menu ===");

//                Console.WriteLine("1. Create Volunteer");
//                Console.WriteLine("2. Read Volunteer by ID");
//                Console.WriteLine("3. Read All Volunteers");
//                Console.WriteLine("4. Update Volunteer");
//                Console.WriteLine("5. Delete Volunteer");
//                Console.WriteLine("6. Delete All Volunteers");
//                Console.WriteLine("0. Back to Main Menu");
//                Console.Write("Select an option: ");

//                int option = int.Parse(Console.ReadLine());

//                switch (option)
//                {
//                    case 1:
//                        CreateVolunteer();
//                        break;

//                    case 2:
//                        ReadVolunteerById();
//                        break;

//                    case 3:
//                        ReadAllVolunteers();
//                        break;

//                    case 4:
//                        UpdateVolunteer();
//                        break;

//                    case 5:
//                        DeleteVolunteer();
//                        break;

//                    case 6:
//                        DeleteAllVolunteers();
//                        break;

//                    case 0:
//                        running = false;
//                        break;

//                    default:
//                        Console.WriteLine("Invalid option, please try again.");
//                        break;
//                }
//            }
//        }

//        static void CreateVolunteer()
//        {
//            Console.WriteLine("Enter Volunteer details:");
//            Console.Write("ID: ");
//            int id = int.Parse(Console.ReadLine());
//            Console.Write("First Name: ");
//            string firstName = Console.ReadLine();
//            Console.Write("Last Name: ");
//            string lastName = Console.ReadLine();
//            Console.Write("Phone Number: ");
//            string phoneNumber = Console.ReadLine();
//            Console.Write("Email: ");
//            string email = Console.ReadLine();
//            Console.Write("Password: ");
//            string password = Console.ReadLine();
//            Console.Write("Address: ");
//            string address = Console.ReadLine();
//            bool active = true;//איך שולחים את הACTIVE?
//            s_dalVolunteer.Create(new Volunteer(id, firstName, lastName, phoneNumber, email, active, password, address));//מה צריך לשלוח בדיוק? הפרויקט הכי לא מובן במדינה!!!
//            Console.WriteLine("Volunteer created successfully!");
//        }

//        static void ReadVolunteerById()
//        {
//            Console.Write("Enter Volunteer ID to read: ");
//            int id = int.Parse(Console.ReadLine());

//            var volunteer = s_dalVolunteer.Read(id);
//            if (volunteer != null)
//            {
//                Console.WriteLine($"Volunteer Found: {volunteer}");
//            }
//            else
//            {
//                Console.WriteLine("Volunteer not found.");
//            }
//        }

//        static void ReadAllVolunteers()
//        {
//            var volunteers = s_dalVolunteer!.ReadAll();
//            Console.WriteLine("All Volunteers:");
//            foreach (var volunteer in volunteers)
//            {
//                Console.WriteLine(volunteer);
//            }
//        }

//static void UpdateVolunteer()//מה לעדכן??????? הפרויקט הכי מחריד בתבל!!פיכסס
//{
//    Console.Write("Enter Volunteer ID to update: ");
//    int id = int.Parse(Console.ReadLine());

//    var volunteer = s_dalVolunteer!.Read(id);
//    if (volunteer != null)
//    {
//        Console.WriteLine($"Current Volunteer: {volunteer}");
//        Console.Write("Enter new name: ");
//        string newName = Console.ReadLine();
//        var updatedVolunteer = volunteer with { FirstName = newName };

//        s_dalVolunteer.Update(updatedVolunteer);
//        Console.WriteLine("Volunteer updated successfully!");
//    }
//    else
//    {
//        Console.WriteLine("Volunteer not found.");
//    }
//}

//        static void DeleteVolunteer()
//        {
//            Console.Write("Enter Volunteer ID to delete: ");
//            int id = int.Parse(Console.ReadLine());

//            s_dalVolunteer!.Delete(id);
//            Console.WriteLine("Volunteer deleted successfully!");
//        }

//        static void DeleteAllVolunteers()
//        {
//            s_dalVolunteer!.DeleteAll();
//            Console.WriteLine("All Volunteers deleted successfully!");
//        }
//    }
//}