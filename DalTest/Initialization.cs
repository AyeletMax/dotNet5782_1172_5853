namespace DalTest;
using DalApi;
using DO;
public static class Initialization
{
    private static IAssignment? s_dalAssignment;
    private static ICall? s_dalCall;
    private static IConfig? s_dalConfig;
    private static IVolunteer? s_dalVolunteer;

    private static readonly Random s_rand = new();
    private static void createVolunteer()
    {
        string[] volunteerNames =
       { "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Shira Israelof" };
        foreach (var name in volunteerNames)
        {
            int id;
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalVolunteer!.Read(id) != null);

            string[] fullName = name.Split(' ');
            string FirstName = fullName[0];
            string LastName = fullName[1];



        }

    }
}
