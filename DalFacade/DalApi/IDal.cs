using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    /// <summary>
    /// Interface for Data Access Layer (DAL) to access entities and configuration.
    /// </summary>
    public interface IDal
    {
        IAssignment Assignment { get; }  // Access to Assignment data.
        ICall Call { get; }              // Access to Call data.
        IVolunteer Volunteer { get; }    // Access to Volunteer data.
        IConfig Config { get; }          // Access to system configuration.
        void ResetDB();                  // Resets the database.
    }
}
