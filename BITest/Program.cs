using System;
using BlApi;
using BO;
using DO;

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

                    if (!int.TryParse(Console.ReadLine(), out int choice))
                        throw new BO.BlInvalidFormatException("The sub menu choice is not valid.");

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
            catch (Exception ex)
            {
                throw new BO.BlGeneralDatabaseException("A custom error occurred in Initialization.", ex);
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
                            if (Enum.TryParse(Console.ReadLine(), out BO.TimeUnit timeUnit))
                            {
                                s_bl.Admin.AdvanceClock(timeUnit);
                                Console.WriteLine("System clock advanced.");
                            }
                            else
                            {
                                Console.WriteLine("Invalid time unit.");
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
                            //אולי צריך לזרוק שגיאה
                            else
                            {
                                Console.WriteLine("Invalid time format.");
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
                    throw new BO.BlGeneralDatabaseException("A custom error occurred in AdminMenu.", ex);
                }
            }
        }

        static void VolunteerMenu()
        {
            //אין את כל הפונקציות של מתנדב
            while (true)
            {
                Console.WriteLine("\n--- Volunteer Management ---");
                Console.WriteLine("1. List Volunteers");
                Console.WriteLine("2. Read Volunteer by ID");
                Console.WriteLine("3. Add Volunteer");
                Console.WriteLine("4. Remove Volunteer");
                Console.WriteLine("0. Back");
                Console.Write("Choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out int choice))
                    throw new BO.BlInvalidFormatException("The volunteer menu choice is not valid.");

                switch (choice)
                {
                    case 1:
                        //צריך לאפשר גם את כל המיון?
                        foreach (var volunteer in s_bl.Volunteer.GetVolunteersList())
                            Console.WriteLine(volunteer);
                        break;
                    case 2:
                        Console.Write("Enter Volunteer ID: ");
                        if (int.TryParse(Console.ReadLine(), out int volunteerId))
                        {
                            var volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                            Console.WriteLine(volunteer);
                        }
                        else
                        //לא צריך לעשות את זה בזריקה?
                        {
                            Console.WriteLine("Invalid ID.");
                        }
                        break;
                    case 3:
                        //להוסיף עוד שדות
                        Console.Write("Enter Full Name: ");
                        string fullName = Console.ReadLine()!;
                        Console.Write("Enter Phone Number: ");
                        string phoneNumber = Console.ReadLine()!;
                        Console.Write("Enter Email: ");
                        string email = Console.ReadLine()!;
                        Console.Write("Enter Role (Manager, Volunteer): ");
                        if (Enum.TryParse(Console.ReadLine(), out BO.Role role))
                        {
                            var volunteer = new BO.Volunteer
                            {
                                Name = fullName,
                                Phone = phoneNumber,
                                Email = email,
                                MyRole = role,
                                Active = true
                            };
                            s_bl.Volunteer.AddVolunteer(volunteer);
                            Console.WriteLine("Volunteer added.");
                        }
                        break;
                    case 4:
                        Console.Write("Enter Volunteer ID: ");
                        if (int.TryParse(Console.ReadLine(), out int vId))
                        {
                            s_bl.Volunteer.DeleteVolunteer(vId);
                            Console.WriteLine("Volunteer removed.");
                        }
                        //לזרוק חריגה אולי
                        else
                        {
                            Console.WriteLine("Invalid ID.");
                        }
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
                        throw new BO.BlInvalidFormatException("The call menu choice is not valid.");

                    switch (choice)
                    {
                        //לא צריך לשלוח ע"פ מה למיין
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
                            //לבדוק האם לזרוק שגיאה
                            else
                            {
                                Console.WriteLine("Invalid ID.");
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
                                }
                            }
                            break;
                        case 4:
                            Console.Write("Enter Call ID: ");
                            if (int.TryParse(Console.ReadLine(), out int cId))
                            {
                                s_bl.Call.DeleteCall(cId);
                                Console.WriteLine("Call removed.");
                            }
                            //לזרוק שגיאה
                            else
                            {
                                Console.WriteLine("Invalid ID.");
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
