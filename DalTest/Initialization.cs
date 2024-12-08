namespace DalTest;
using DalApi;
using DO;
using System.Data;

public static class Initialization
{
    private static IDal? s_dal; // The DAL interface instance for interacting with the database.

    private static readonly Random s_rand = new(); // Random generator for creating random data.

    /// <summary>
    /// Creates a list of volunteers with random data.
    /// The first volunteer is predefined as a manager, and the rest are standard volunteers with random details.
    /// </summary>
    private static void createVolunteer()
    {
        string[] FullNames = {
            "Lior Ben-David", "Batya Hirsch", "Shimon Farkash", "Tzvi Berkovitz", "Meir Goldfarb",
            "Bat-Chen Shalom", "Yonatan Spector", "Erez Gur", "Gal Zohar", "Rachel Nachman",
            "Yarden Levy", "Doron Klein", "Yulia Shaked", "Hila Shulman" ,"Avigail Frankel"
        };
        string[] phones = { "0501234567", "0529876543", "0541239876", "0559871234", "0533217654",
            "0504561239","0548551329","0548425138","0583282450","0583232333","0548482197",
            "0583265120","0548533173","0583292455","0583262639"
        };
        string[] passwords = { "pass123", "eli2023", "yair007", "ariela456", "dina789", "shira101",
            "123asd","a1b2c3","hello123","AaBbCc112233","abc123!@#","meMyselfAndI","ayelet@@@234","951A","HiHello!!"
        };
        string[] addresses = {
                "9 Weizman St, Tel Aviv, Israel", "40 Jabotinsky St, Petah Tikva, Israel", "Ein Kerem 5, Jerusalem, Israel",
                "Tel Hashomer 6, Ramat Gan, Israel", "20 Yirmiyahu St, Jerusalem, Israel","128 Kaplan St, Tel Aviv, Israel",
                "65 Jabotinsky St, Kfar Saba, Israel","Barzilai 6, Ashkelon, Israel", "Ziv 6, Safed, Israel", "11 Beilinson St, Tel Aviv, Israel",
                "13 Azrieli St, Netanya, Israel", "2 Kalay St, Haifa, Israel", "11 Wolfson St, Holon, Israel",
                 "12 Bialik St, Ashdod, Israel", "17 Heichal Shlomo St, Haifa, Israel"
        };
        double[] longitudes = {
               34.7800, 34.8778, 35.2037, 34.8011, 35.1804, 34.7707,
               34.8999, 34.5675, 35.4884, 34.7746, 34.8446, 34.9796,
               34.7673, 34.6414, 34.9762
        };
        double[] latitudes = {
               32.1094, 32.0850, 31.7732, 32.0735, 31.7683, 32.0810,
               32.1820, 31.6700, 32.9674, 32.0840, 32.3143, 32.7872,
               32.0167, 31.8015, 32.7682
        };

        // Adding a predefined manager.
        s_dal!.Volunteer.Create(new Volunteer(s_rand.Next(200000000, 400000000), "Chen", "Cohen", "0583265482", "chen@gmail.com", true, Role.Manager, "chen!123", "Zhbotinski 15", 32.1, 32.8, 10));

        // Adding volunteers with random details.
        for (int i = 0; i < FullNames.Length; i++)
        {
            string[] nameParts = FullNames[i].Split(' ');
            s_dal!.Volunteer!.Create(new Volunteer(s_rand.Next(200000000, 400000000), nameParts[0], nameParts[1], phones[i], $"{phones[i]}@gmail.com", true, Role.Volunteer, passwords[i], addresses[i],
                latitudes[i], longitudes[i], s_rand.Next(0, 8)));
        }
    }

    /// <summary>
    /// Creates a list of calls with random details.
    /// Each call includes a type, address, coordinates, and start/end times.
    /// </summary>
    private static void createCall()
    {
        string[] verbalDescriptions = {
        "Live performance for patients in the day care department",
        "Soothing music for a patient after surgery",
        "Singing and emotional activity for a bedridden patient",
        "Tea break and singing with patients in the rehabilitation department",
        "Personalized music playlist for patients in recovery",
        "Singing performance together with patients and caregivers",
        "Music therapy session for post-stroke rehabilitation patients",
        "Singing and reminiscence activity for seniors in a care home",
        "Community sing-along event for patients and families",
        "Songwriting therapy session for patients dealing with grief",
        };
        CallType[] callTypes =
        {
             CallType.MusicPerformance,
             CallType.MusicTherapy,
             CallType.SingingAndEmotionalSupport,
             CallType.GroupActivities,
             CallType.PersonalizedMusicCare,
             CallType.MusicPerformance,
             CallType.MusicTherapy,
             CallType.SingingAndEmotionalSupport,
             CallType.GroupActivities,
             CallType.PersonalizedMusicCare,
        };
        double[] longitudes = { /* array of longitudes */ };
        double[] latitudes = { /* array of latitudes */ };
        string[] addresses = { /* array of addresses */ };

        // Generate calls with random data.
        int hour = Math.Max(0, s_dal!.Config.Clock.Hour - 5);
        DateTime begin = new DateTime(s_dal!.Config.Clock.Year, s_dal.Config.Clock.Month, s_dal.Config.Clock.Day, hour, 0, 0);
        int range = Math.Max(0, (int)(s_dal.Config.Clock - begin).TotalMinutes);

        for (int i = 0; i < 50; i++)
        {
            int startTime = s_rand.Next(range);
            int randIndex = s_rand.Next(verbalDescriptions.Length);
            s_dal!.Call.Create(new Call(0, callTypes[randIndex], addresses[randIndex], latitudes[i], longitudes[i], begin.AddMinutes(startTime), begin.AddMinutes(startTime + s_rand.Next(30, 360)), verbalDescriptions[randIndex]));
        }
    }

    /// <summary>
    /// Assigns volunteers to calls, creating assignments with random timing within valid ranges.
    /// </summary>
    private static void createAssignment()
    {
        List<Volunteer>? volunteers = s_dal!.Volunteer.ReadAll().ToList();
        List<Call>? calls = s_dal!.Call.ReadAll().ToList();

        for (int i = 0; i < 50; i++)
        {
            int callId = calls[i].Id;
            int volunteerId = volunteers[s_rand.Next(volunteers.Count)].Id;

            DateTime minTime = calls[i].OpenTime;
            DateTime maxTime = (DateTime)calls[i].MaxFinishTime!;
            TimeSpan difference = maxTime - minTime - TimeSpan.FromHours(2);
            int validDifference = (int)Math.Max(difference.TotalMinutes, 0);
            DateTime randomTime = minTime.AddMinutes(s_rand.Next(validDifference));

            s_dal!.Assignment.Create(new Assignment(0, callId, volunteerId, randomTime, randomTime.AddHours(2),
                (FinishCallType)s_rand.Next(Enum.GetValues(typeof(FinishCallType)).Length - 1)));
        }
    }

    /// <summary>
    /// Initializes the database by resetting data and populating it with volunteers, calls, and assignments.
    /// </summary>
    /// <param name="dal">The DAL instance to interact with the database.</param>
    public static void DO(IDal dal)
    {
        s_dal = dal ?? throw new NullReferenceException("DAL object cannot be null!");

        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();

        Console.WriteLine("Initializing Volunteers list...");
        createVolunteer();
        Console.WriteLine("Initializing Calls list...");
        createCall();
        Console.WriteLine("Initializing Assignments list...");
        createAssignment();
    }
}
