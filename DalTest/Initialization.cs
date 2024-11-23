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
            "Avi Cohen", "Yosef Levy", "Miriam Katz", "Rivka Friedman", "David Shapiro",
            "Sarah Goldstein", "Moshe Weiss", "Esther Kaplan", "Chaim Rosenberg", "Leah Stein",
            "Shlomo Adler", "Rachel Baruch", "Yaakov Cohen", "Tamar Goldman", "Eli Rubin",
            "Chaya Mizrahi", "Benjamin Klein", "Noa Cohen", "Avraham Ben-David", "Dalia Shlomo",
            "Isaac Greenberg", "Yaara Weissman", "Ziv Tzukrel", "Sophie Abramov", "Reuven Katz",
            "Hannah Rosen", "Yitzhak Levi", "Shira Peretz", "Nadav Shtern", "Naomi Biton",
            "Simcha Azulay", "Daniel Rosen", "Adina Raskin", "Eliezer Yeger", "Maya Erez",
            "Avigail Frankel", "Yitzhak Polak", "Tzafira Ben-Shimon", "Eliyahu Halimi", "Hadar Levy",
            "Lior Ben-David", "Batya Hirsch", "Shimon Farkash", "Tzvi Berkovitz", "Meir Goldfarb",
            "Bat-Chen Shalom", "Yonatan Spector", "Erez Gur", "Gal Zohar", "Rachel Nachman",
            "Yarden Levy", "Doron Klein", "Yulia Shaked", "Hila Shulman" };

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
        //string[] verbalDescriptions = {
        //     "Singing for a patient in the internal medicine department",
        //     "Guitar playing for a patient in isolation",
        //     "Singing and emotional activity for a bedridden patient",
        //     "Live performance for patients in the day care department",
        //     "Tea break and singing with patients in the rehabilitation department",
        //     "Soothing music for a patient after surgery",
        //     "Late night singing after treatment",
        //     "Singing performance together with patients and caregivers",
        //     "Musical activity for children with cancer"};
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
        "Live concert for patients in the oncology unit",
        "Therapeutic music session for elderly patients with dementia",
        "Singing and relaxation activity for patients with chronic pain",
        "Group drumming and singing for patients in group therapy",
        "Music therapy to reduce anxiety for patients undergoing chemo",
        "Guitar playing for a patient in isolation",
        "Interactive music therapy for mental health patients",
        "Singing circle for elderly patients with Alzheimer's disease",
        "Singing circle with patients and staff in the pediatric ward",
        "Music and mindfulness session for patients with insomnia",
        "Singing for a patient in the internal medicine department",
        "Relaxing harp music for patients in the surgical recovery area",
        "Singing and emotional support for caregivers in the ICU",
        "Evening music session for patients in the rehabilitation center",
        "Guided music meditation for patients experiencing pain",
        "Acoustic guitar session for patients in the psychiatric ward",
        "Soothing instrumental music for intensive care unit patients",
        "Singing and yoga session for post-surgery patients",
        "Dance and sing-along activity for patients in long-term care",
        "Songwriting therapy for children with chronic illness",
        "Late night singing after treatment",
        "Music therapy for patients with traumatic brain injury",
        "Singing for children in the oncology ward",
        "Group choir performance for patients in the hospice care unit",
        "Personalized musical support for physical therapy patients",
        "Piano performance to calm patients during emergency treatments",
        "Therapeutic drumming circle for cancer patients",
        "Singing and art therapy for children with chronic illness",
        "Interactive sing-along for patients in the hospice care unit",
        "Songs and emotional support for caregivers in the ICU",
        "Singing performance for patients in the cardiac care unit",
        "Interactive vocal exercises for patients with breathing problems",
        "Singing for patients in the outpatient care unit",
        "Choir performance for patients in the rehabilitation department"};
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
               CallType.MusicPerformance,
               CallType.MusicTherapy,
               CallType.SingingAndEmotionalSupport,
               CallType.GroupActivities,
               CallType.PersonalizedMusicCare,
               CallType.MusicPerformance,
               CallType.MusicTherapy,
               CallType.SingingAndEmotionalSupport,
               CallType.GroupActivities,
               CallType.PersonalizedMusicCare
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

        for (int i = 0; i < 50; i++)
        {
            CallType MyCallType = callTypes[i];
            //CallType MyCallType = (CallType)s_rand.Next(0, Enum.GetValues(typeof(CallType)).Length);
            string VerbalDescription = verbalDescriptions[i];
            string Address = addresses[i];
            double latitude = latitudes[i];
            double longitude = longitudes[i];
            //עשינו את שעת פתיחה טוב?
            DateTime OpenTime = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1);
            DateTime MaxFinishTime = OpenTime.AddDays(s_rand.Next((s_dalConfig.Clock - OpenTime).Days));
            //מה עושים עם הID רץ?
            s_dalCall!.Create(new(0, MyCallType, VerbalDescription, Address, latitude, longitude, OpenTime, MaxFinishTime));
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