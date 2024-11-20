namespace DalTest;
using DalApi;
using DO;
using System.Data;

public static class Initialization
{
    private static IAssignment? s_dalAssignment;
    private static ICall? s_dalCall;
    private static IConfig? s_dalConfig;
    private static IVolunteer? s_dalVolunteer;

    private static readonly Random s_rand = new();
    private static void createVolunteer()
    {
        string[] firstNames = { "Dani", "Eli", "Yair", "Ariela", "Dina", "Shira" };
        string[] lastNames = { "Levy", "Amar", "Cohen", "Levin", "Klein", "Israelof" };
        string[] emails = { "dani@gmail.com", "eli@gmail.com", "yair@gmail.com", "ariela@gmail.com", "dina@gmail.com", "shira@gmail.com" };
        string[] phones = { "0501234567", "0529876543", "0541239876", "0559871234", "0533217654", "0504561239" };
        string[] passwords = { "pass123", "eli2023", "yair007", "ariela456", "dina789", "shira101" };
        string[] addresses = { "Tel Aviv", "Jerusalem", "Haifa", "Eilat", "Rishon Lezion", "Beer Sheva" };

        for (int i = 0; i < firstNames.Length; i++)
        {
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalVolunteer!.Read(id) != null);
            string FirstName = firstNames[i];
            string LastName = lastNames[i];
            string Phone = phones[i];
            string Email = emails[i];
            bool Active = true;//האם הוא פעיל
            string Password = passwords[i];
            string Address = addresses[i];
            double latitude = s_rand.NextDouble() * (33.0 - 29.5) + 29.5;
            double longitude = s_rand.NextDouble() * (35.9 - 34.2) + 34.2;
            Role MyRole = (Role)s_rand.Next(0, Enum.GetValues(typeof(Role)).Length);
            double largestDistance = s_rand.Next(5, 50);
            DistanceType MyDistanceType = (DistanceType)s_rand.Next(0, Enum.GetValues(typeof(DistanceType)).Length);
            s_dalVolunteer!.Create(new Volunteer(id, FirstName, LastName, Phone, Email, Active, Password, Address, latitude,
                longitude, MyRole, largestDistance, MyDistanceType));
        }
    }
    private static void createAssignment()
    {
        var volunteers = s_dalVolunteer!.ReadAll();  
        //כמה קריאות צריך עשינו 15
        for (int i = 0; i < 15; i++)
        {
            int VolunteerId = volunteers[s_rand.Next(volunteers.Count)].Id;
            DateTime EntranceTime=new DateTime(s_dalConfig.Clock.Year - 2, 1, 1);
            DateTime ExitTime= EntranceTime.AddDays(s_rand.Next((s_dalConfig.Clock - EntranceTime).Days));
            FinishCallType FinishCallType=(FinishCallType)s_rand.Next(0, Enum.GetValues(typeof(FinishCallType)).Length);
            //לא יודעות איך לשלוח את הID רץ
            s_dalAssignment!.Create(new(0, VolunteerId, EntranceTime, ExitTime, FinishCallType));
        }
    }
    private static void createCall()
    {
        string[] verbalDescriptions = {"Singing for a patient in the internal medicine department",
             "Guitar playing for a patient in isolation",
             "Singing and emotional activity for a bedridden patient",
             "Live performance for patients in the day care department",
             "Tea break and singing with patients in the rehabilitation department",
             "Soothing music for a patient after surgery",
             "Late night singing after treatment",
             "Singing performance together with patients and caregivers",
             "Musical activity for children with cancer"};
        string[] addresses = { "Tel Aviv, Israel", "Jerusalem, Israel", "Haifa, Israel", "Eilat, Israel", "Rishon Lezion, Israel", "Beer Sheva, Israel" };

        //כמה קריאות צריך? עשינו 15
        for (int i = 0; i < 15; i++)
        {
            CallType MyCallType = (CallType)s_rand.Next(0, Enum.GetValues(typeof(CallType)).Length);
            string VerbalDescription = verbalDescriptions[i];
            string Address = addresses[i];
            double Latitude = s_rand.NextDouble() * (33.5 - 29.5) + 29.5;
            double Longitude = s_rand.NextDouble() * (35.9 - 34.2) + 34.2;
            //עשינו את שעת פתיחה טוב?
            DateTime OpenTime= new DateTime(s_dalConfig.Clock.Year - 2, 1, 1);
            DateTime MaxFinishTime = OpenTime.AddDays(s_rand.Next((s_dalConfig.Clock - OpenTime).Days));
            //מה עושים עם הID רץ?
            s_dalCall!.Create(new(0,MyCallType, VerbalDescription, Address, Latitude, Longitude, OpenTime, MaxFinishTime));
        }
    }

    public static void DO(IAssignment? dalAssignment, ICall? dalCall, IConfig? dalConfig, IVolunteer? dalVolunteer)
    {
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL can not be null!");
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DAL can not be null!");
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL can not be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset();
        s_dalVolunteer.DeleteAll();
        s_dalCall.DeleteAll();
        s_dalAssignment.DeleteAll();

        Console.WriteLine("Initializing Volunteers list ...");
        createVolunteer();
        Console.WriteLine("Initializing Assignments list ...");
        createAssignment();
        Console.WriteLine("Initializing Calls list ...");
        createCall();
    }
};





    

      