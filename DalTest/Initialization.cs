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
        //לשנות את המשתנים שיעודכנו מהמערך הזה
        string[] FullNames = {
            "Lior Ben-David", "Batya Hirsch", "Shimon Farkash", "Tzvi Berkovitz", "Meir Goldfarb",
            "Bat-Chen Shalom", "Yonatan Spector", "Erez Gur", "Gal Zohar", "Rachel Nachman",
            "Yarden Levy", "Doron Klein", "Yulia Shaked", "Hila Shulman" ,"Avigail Frankel" };

        string[] phones = { "0501234567", "0529876543", "0541239876", "0559871234", "0533217654",
            "0504561239","0548551329","0548425138","0583282450","0583232333","0548482197",
            "0583265120","0548533173","0583292455","0583262639" };
        string[] passwords = { "pass123", "eli2023", "yair007", "ariela456", "dina789", "shira101",
            "123asd","a1b2c3","hello123","AaBbCc112233","abc123!@#","meMyselfAndI","ayelet@@@234","951A","HiHello!!" };
        //להוסיף עוד כתובות מדויקות
        string[] addresses = { "Tel Aviv", "Jerusalem", "Haifa", "Eilat", "Rishon Lezion", "Beer Sheva" };
        //להוסיף מערכים של קוי אורך ורוחב
        double[] latitudes = { };
        double[] longitudes = { };
        s_dalVolunteer!.Create(new(s_rand.Next(200000000, 400000000), "Chen", "Cohen", "0583265482", "chen@gmail.com", true, Role.Manager, "chen!123", "Zhbotinski 15", 32.1, 32.8, 10));
        for (int i = 0; i < FullNames.Length; i++)
        {
            string[] nameParts = FullNames[i].Split(' ');
            s_dalVolunteer!.Create(new Volunteer(s_rand.Next(200000000, 400000000), nameParts[0], nameParts[1], phones[i], $"{phones[i]}@gmail.com", true, Role.Volunteer, passwords[i], addresses[i],
                latitudes[i], longitudes[i], s_rand.Next(0, 8)));
        }
    }

    //חובה להבין!!!!
    private static void createAssignment()
    {
        List<Volunteer>? volunteers = s_dalVolunteer!.ReadAll();
        List<Call>? calls = s_dalCall!.ReadAll();
        DateTime startTime = new DateTime(s_dalConfig.Clock.Year, s_dalConfig.Clock.Month, s_dalConfig.Clock.Day, s_dalConfig.Clock.Hour - 5, 0, 0);
        //כמה קריאות צריך עשינו 15
        for (int i = 0; i < 50; i++)
        {
            DateTime minTime = calls[i].OpenTime;
            DateTime maxTime = (DateTime)calls[i].MaxFinishTime!;
            TimeSpan difference = maxTime - minTime - TimeSpan.FromHours(2);
            DateTime randomTime = minTime.AddMinutes(s_rand.Next((int)difference.TotalMinutes));

            s_dalAssignment!.Create(new Assignment(
                calls[i].Id,
                volunteers[s_rand.Next(calls.Count)].Id,
                randomTime,
                randomTime.AddHours(2),
                (FinishCallType)s_rand.Next(Enum.GetValues(typeof(FinishCallType)).Length - 1)));
        }
    }
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
        double[] longitudes = {
            34.7867, 34.8045, 34.8067, 34.7895, 34.7960,
            34.7775, 34.7729, 34.7915, 34.8206, 34.7769,
            34.7618, 34.7462, 34.7213, 34.7030, 34.7225,
            34.8182, 34.7879, 34.7571, 34.7898, 34.7509,
            34.8252, 34.7740, 34.7306, 34.7442, 34.7564,
            34.7549, 34.7326, 34.7718, 34.7864, 34.7446,
            34.7365, 34.7809, 34.7989, 34.7588, 34.7545,
            34.7916, 34.7919, 34.7970, 34.8047, 34.8061,
            34.8118, 34.8051, 34.7744, 34.8147, 34.7823,
            34.7469, 34.7776, 34.7995, 34.7627, 34.8148 };
        double[] latitudes = {
            32.1098, 32.0869, 32.0727, 32.0715, 32.0968,
            32.0805, 32.1027, 32.0972, 32.0783, 32.1019,
            32.0916, 32.0744, 32.0731, 32.0915, 32.0818,
            32.0771, 32.1013, 32.1036, 32.0863, 32.1078,
            32.0789, 32.0934, 32.1047, 32.0787, 32.0851,
            32.1033, 32.0781, 32.0861, 32.0951, 32.1083,
            32.0875, 32.0785, 32.0891, 32.0978, 32.0842,
            32.1015, 32.0994, 32.0950, 32.0840, 32.0909,
            32.0920, 32.0955, 32.0803, 32.0877, 32.0774,
            32.0979, 32.0905, 32.0856, 32.0912, 32.1074  };
        string[] addresses = { "Tel Aviv, Israel", "Jerusalem, Israel", "Haifa, Israel", "Eilat, Israel", "Rishon Lezion, Israel", "Beer Sheva, Israel" };
        DateTime begin = new DateTime(s_dalConfig.Clock.Year, s_dalConfig.Clock.Month, s_dalConfig.Clock.Day, s_dalConfig.Clock.Hour - 5, 0, 0);
        int range = (int)(s_dalConfig.Clock - begin).TotalMinutes;
        for (int i = 0; i < 50; i++)
        {
            int startTime = s_rand.Next(range);
            int randIndex = s_rand.Next(verbalDescriptions.Length);
            s_dalCall!.Create(new Call(callTypes[randIndex], addresses[i], latitudes[i], longitudes[i], begin.AddMinutes(startTime), begin.AddMinutes(startTime + s_rand.Next(30, 360)), verbalDescriptions[randIndex]));
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
}