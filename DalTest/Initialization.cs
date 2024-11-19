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
}
