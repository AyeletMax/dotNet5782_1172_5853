using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Dal;
using DalApi;

/// Provides an implementation of the IDal interface for managing data access operations.
sealed internal class DalList : IDal
{
    public static IDal Instance { get; } = new DalList();
    private DalList() { }
    /// Provides access to assignment data operations.
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    /// Provides access to call data operations.
    public ICall Call { get; } = new CallImplementation();

    /// Provides access to volunteer data operations.
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();

    /// Provides access to configuration settings.
    public IConfig Config { get; } = new ConfigImplementation();

   /// Resets the entire database, clearing all assignments, calls, volunteers, and configurations.
    public void ResetDB()
    {
        Assignment.DeleteAll();
        Call.DeleteAll();
        Volunteer.DeleteAll();
        Config.Reset();
    }
}
