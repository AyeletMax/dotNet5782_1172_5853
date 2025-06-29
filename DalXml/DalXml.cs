using DalApi;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Dal;
/// <summary>
/// Implements the IDal interface for accessing and manipulating data in XML files.
/// Provides data operations for Assignments, Calls, Volunteers, and Config.
/// </summary>

sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml();
    private DalXml() { }
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Resets the database by deleting all assignments, calls, and volunteers, 
    /// and resetting the system configuration.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void ResetDB()
    {
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}
