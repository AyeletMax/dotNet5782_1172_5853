namespace DalTest;
using DalApi;
using DO;
using System.Data;
using System.Security.Cryptography;
using System.Text;

public static class Initialization
{
    private static IDal? s_dal;
    private static readonly Random s_rand = new();
    private static void createVolunteer()
    {
        string[] FullNames = {
        "Lior Ben-David", "Batya Hirsch", "Shimon Farkash", "Tzvi Berkovitz", "Meir Goldfarb",
        "Bat-Chen Shalom", "Yonatan Spector", "Erez Gur", "Gal Zohar", "Rachel Nachman",
        "Yarden Levy", "Doron Klein", "Yulia Shaked", "Hila Shulman", "Avigail Frankel",
        "Noam Cohen", "Tamar Shapira", "Itai Levi", "Shira Katz", "Elad Regev",
        "Maya Azulay", "Omer Harel", "Tal Hadad", "Yael Avraham", "Amit Weiss",
        "Eden Peretz", "Nir Barkai", "Adi Tal", "Roni Geva", "Lior Maimon",
        "Dana Michaeli", "Gilad Peleg", "Alon Zohar", "Bar Shani", "Inbar Halevi",
        "Yossi Golan", "Hadar Shitrit", "Liat Mor", "David Azulai", "Noga Dahan",
        "Sarit Levy", "Eyal Ron", "Ziv Nir", "Talia Ben-Ami", "Ronit Barak",
        "Lina Cohen", "Ariel Morad", "Or Shahar", "Talya Stern", "Galit Mashiach"       
        };
        string[] phones = { "0501234567", "0529876543", "0541239876", "0559871234", "0533217654",
        "0504561239", "0548551329", "0548425138", "0583282450", "0583232333",
        "0548482197", "0583265120", "0548533173", "0583292455", "0583262639",
        "0521122334", "0534455667", "0509988776", "0543322110", "0523344556",
        "0502233445", "0556677889", "0537788990", "0584433221", "0541122113",
        "0523344567", "0538899776", "0547766554", "0506677885", "0524455667",
        "0539988776", "0505566778", "0547788992", "0551122334", "0503322114",
        "0526677880", "0534455990", "0546655443", "0508877665", "0583322445",
        "0547766885", "0502299443", "0529933771", "0531144775", "0554433211",
        "0547766901", "0583221100", "0501100223", "0525544332", "0549988776"
        };
        string[] passwords = {
            "A1b@D2eF", "X3y#Z9aH", "P@ssw0rD1", "T#4oP@r7", "M1n#0F8z", "G#tX9jQ2", "L8!mT9uB", "D9v@Zq7X", "B5a#cG3e", "K#p9J1xQ",
        "Q8@tW5oP", "R3v@C9fX", "S4z#D8oY", "N7f#L9pB", "C3uZ#hT1", "A2b@C3dE", "X9y#T1uV", "Z@xP7qR5", "M#n1B2v3", "H1j#K2lM",
        "P@L3mN8z", "G#W9xQ2z", "L0!uN8rA", "C9v@Zq1X", "B7a#fD3e", "K#z2J9xP", "E8@vA5mT", "T3v@R9fL", "S5z#H2oB", "Y7f#J1pB",
        "U3rZ#tT1", "D3t@X2yL", "M1c#L0vZ", "A9k#U2oB", "Q1n@W4eP", "B3r#C5fX", "S4x@L8yD", "V7z#N3pT", "J1t#H9vR", "E4u@Y6oL",
        "P5x#Z1qB", "L2z@N4eM", "D9q#R7uF", "C8o@T1wY", "M6j#K3pB", "F3u@G9xT", "Z7v#H5qL", "B1t@Y8nO", "K2f#W3mR", "N9p@C7eX"
        };

        string[] addresses = {
            "9 Weizman St, Tel Aviv", "40 Jabotinsky St, Petah Tikva", "5 Ein Kerem St, Jerusalem",
            "6 Tel Hashomer St, Ramat Gan",  "20 Yirmiyahu St, Jerusalem",  "128 Kaplan St, Tel Aviv",
            "65 Jabotinsky St, Kfar Saba", "6 Barzilai St, Ashkelon",  "6 Ziv St, Safed",
            "11 Beilinson St, Tel Aviv",  "13 Azrieli St, Netanya","2 Kalay St, Haifa",  "11 Wolfson St, Holon",
            "12 Bialik St, Ashdod",  "17 Heichal Shlomo St, Haifa","3 HaPalmach St, Tel Aviv", "5 Herzl St, Jerusalem",
            "7 HaNegev St, Be'er Sheva", "15 Harav Kook St, Bnei Brak","22 Sheshet Hayamim St, Holon","26 Even Gvirol St, Ramat Gan",
            "29 Begin Blvd, Petah Tikva","18 Menachem Begin Rd, Hadera","8 HaHistadrut St, Bat Yam", "34 Levi Eshkol Blvd, Haifa",
            "16 Moshe Dayan St, Lod", "7 Ben Gurion St, Rishon LeZion","25 David Remez St, Netanya", "19 Yehuda Halevi St, Ramat Hasharon",
            "4 Pinsker St, Kiryat Ono",  "14 Yitzhak Sadeh St, Tiberias","6 Bar Ilan St, Jerusalem", "12 Hillel St, Nahariya",
            "5 Allenby St, Tel Aviv", "3 Rambam St, Acre","11 HaDekel St, Modiin", "21 Ahad Ha'am St, Kfar Saba",
            "23 Dov Hoz St, Holon", "30 Trumpeldor St, Beit Shemesh","33 Hanassi St, Hadera", "2 Herzl St, Rehovot",
            "8 HaPalmach St, Afula", "10 Jabotinsky St, Eilat","6 Beit Hillel St, Beersheba",  "5 HaRav Herzog St, Givatayim",
            "7 Menorah St, Ashkelon","9 Hashalom Rd, Ra'anana","13 Nordau Blvd, Herzliya","4 Yehuda St, Bat Yam",
            "6 Tchernichovsky St, Ramat Gan"        
        };
        double[] latitudes = {
            32.0853, 32.0889, 31.7732, 32.0735, 31.7683, 32.0810, 32.1803, 31.6700, 32.9674, 32.0840,
            32.3215, 32.7940, 32.0167, 31.8015, 32.7940, 32.0853, 31.7719, 31.2522, 32.0833, 32.0167,
            32.0680, 32.0889, 32.4370, 32.0171, 32.7940, 31.9510, 31.9730, 32.3215, 32.1460, 32.0500,
            32.7922, 31.7719, 33.0076, 32.0671, 32.9236, 31.8980, 32.1803, 32.0167, 31.7480, 32.4370,
            31.8948, 32.6070, 29.5581, 31.2518, 32.0723, 31.6695, 32.1844, 32.1663, 32.0171, 32.0680        
        };
        double[] longitudes = {
            34.7818, 34.8864, 35.2037, 34.8011, 35.1804, 34.7707, 34.9066, 34.5715, 35.4884, 34.7746,
            34.8532, 34.9896, 34.7673, 34.6414, 34.9896, 34.7818, 35.2170, 34.7913, 34.8333, 34.7667,
            34.8240, 34.8864, 34.9196, 34.7454, 34.9896, 34.8882, 34.7925, 34.8532, 34.8390, 34.8500,
            35.5312, 35.2170, 35.0943, 34.7672, 35.0717, 35.0180, 34.9066, 34.7667, 34.9887, 34.9196,
            34.8093, 35.2890, 34.9519, 34.7913, 34.8120, 34.5715, 34.8708, 34.8433, 34.7454, 34.8240
        };
        for (int i = 0; i < FullNames.Length; i++)
        {
            s_dal!.Volunteer!.Create(new Volunteer(GenerateValidIsraeliId(s_rand), FullNames[i], phones[i], $"{phones[i]}@gmail.com", true, Role.Volunteer,
                latitudes[i], longitudes[i], s_rand.Next(0, 8), EncryptPassword(passwords[i]), addresses[i] ));
        }
    }
    private static int GenerateValidIsraeliId(Random rand)
    {
        int idWithoutCheckDigit = rand.Next(10000000, 99999999); 
        int checkDigit = CalculateIsraeliIdCheckDigit(idWithoutCheckDigit);
        int finalId = unchecked(idWithoutCheckDigit * 10 + checkDigit); 

        return finalId >= 0 ? finalId : -finalId; 
    }

    private static int CalculateIsraeliIdCheckDigit(int idWithoutCheckDigit)
    {
        int sum = 0;
        for (int i = 0; i < 8; i++)
        {
            int digit = (idWithoutCheckDigit / (int)Math.Pow(10, 7 - i)) % 10;
            int weighted = digit * (i % 2 == 0 ? 1 : 2);
            sum += (weighted > 9) ? weighted - 9 : weighted;
        }
        return (10 - (sum % 10)) % 10;
    }
    internal static string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password)); ;
        return Convert.ToBase64String(hashedBytes!);
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
            34.7469, 34.7776, 34.7995, 34.7627, 34.8148 
        };
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
            32.0979, 32.0905, 32.0856, 32.0912, 32.1074  
        };
        string[] addresses = { "Tel Aviv, Israel", "Jerusalem, Israel", "Haifa, Israel", "Eilat, Israel", "Rishon Lezion, Israel", "Beer Sheva, Israel",
            "Tel Aviv, Israel", "Jerusalem, Israel", "Haifa, Israel", "Eilat, Israel"};
        int hour = Math.Max(0, s_dal!.Config.Clock.Hour - 5);
        DateTime begin = new DateTime(s_dal!.Config.Clock.Year, s_dal.Config.Clock.Month, s_dal.Config.Clock.Day, hour, 0, 0);
        int range = Math.Max(0, (int)(s_dal.Config.Clock - begin).TotalMinutes);
        for (int i = 0; i < 50; i++)
        {
            int startTime = s_rand.Next(range);
            int randIndex = s_rand.Next(verbalDescriptions.Length); // 0-9
            int addressIndex = randIndex % addresses.Length; // תמיד בטווח 0-9
            s_dal!.Call.Create(new Call(
                callTypes[randIndex],
                addresses[addressIndex],
                latitudes[i],
                longitudes[i],
                begin.AddMinutes(startTime),
                begin.AddMinutes(startTime + s_rand.Next(30, 360)),
                verbalDescriptions[randIndex]
            ));
        }
    }
    private static void createAssignment()
    {
        List<Volunteer>? volunteers = s_dal!.Volunteer.ReadAll().ToList(); ;
        List<Call>? calls = s_dal!.Call.ReadAll().ToList(); ;
        for (int i = 0; i < 50; i++)
        {
            int callId = calls[i].Id;
            int volunteerId = volunteers[s_rand.Next(volunteers.Count)].Id;
            DateTime minTime = calls[i].OpenTime;
            DateTime maxTime = (DateTime)calls[i].MaxFinishTime!;
            TimeSpan difference = maxTime - minTime - TimeSpan.FromHours(2);
            int validDifference = (int)Math.Max(difference.TotalMinutes, 0);
            DateTime randomTime = minTime.AddMinutes(s_rand.Next(validDifference));
            s_dal!.Assignment.Create(new Assignment(callId, volunteerId, randomTime, null,
                (FinishCallType)s_rand.Next(Enum.GetValues(typeof(FinishCallType)).Length - 1)));
        }
    }
  
    //public static void DO(IDal dal)//stage 2
    public static void DO()//stage 4
    {

        //s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!"); //stage 2
        s_dal = DalApi.Factory.Get;
        Console.WriteLine("Reset Configuration values and List values...");
        s_dal.ResetDB();
        Console.WriteLine("Initializing Volunteers list ...");
        createVolunteer();
        Console.WriteLine("Initializing Calls list ...");
        createCall();
        Console.WriteLine("Initializing Assignments list ...");
        createAssignment();
    }
}