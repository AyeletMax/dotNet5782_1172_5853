using DalApi;

namespace Dal;
/// <summary>
/// Implements the IDal interface for accessing and manipulating data in XML files.
/// Provides data operations for Assignments, Calls, Volunteers, and Config.
/// </summary>

sealed public class DalXml : IDal
{
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    public ICall Call { get; } = new CallImplementation();

    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    public IConfig Config { get; } = new ConfigImplementation();
    
    /// <summary>
    /// Resets the database by deleting all assignments, calls, and volunteers, 
    /// and resetting the system configuration.
    /// </summary>
    public void ResetDB()
    {
        Assignment.DeleteAll();
        Volunteer.DeleteAll();
        Call.DeleteAll();
        Config.Reset();
    }
}
