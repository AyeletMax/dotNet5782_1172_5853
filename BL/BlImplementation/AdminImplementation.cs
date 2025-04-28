using BlApi;
using BO;
using DalApi;
using Helpers;

namespace BlImplementation;
/// <summary>
/// Implementation of the admin interface, including clock management, risk range management, and database management.
/// </summary>
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// Returns the current system clock time.
    /// </summary>
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }
    /// <summary>
    /// Advances the system clock by the specified time unit.
    /// </summary>
    /// <param name="unit">The time unit to add (minute, hour, day, month, year).</param>
    public void AdvanceClock(TimeUnit unit)
    {
        DateTime newClock = unit switch
        {
            TimeUnit.MINUTE => AdminManager.Now.AddMinutes(1),
            TimeUnit.HOUR => AdminManager.Now.AddHours(1),
            TimeUnit.DAY => AdminManager.Now.AddDays(1),
            TimeUnit.MONTH => AdminManager.Now.AddMonths(1),
            TimeUnit.YEAR => AdminManager.Now.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), "Invalid time unit")
        };

        AdminManager.UpdateClock(newClock);
    }
    /// <summary>
    /// Returns the maximum risk range in the system.
    /// </summary>
    public TimeSpan GetMaxRange()
    {
        return AdminManager.RiskRange;
    }
    /// <summary>
    /// Sets the maximum risk range in the system.
    /// </summary>
    /// <param name="maxRange">The new risk range to be set.</param>
    public void SetMaxRange(TimeSpan maxRange)
    {
        AdminManager.RiskRange = maxRange;
    }
    /// <summary>
    /// Resets the database and system clock.
    /// </summary>
    public void ResetDB()
    {
        _dal.ResetDB();
        AdminManager.Reset();
    }
    /// <summary>
    /// Initializes the database and updates the system clock.
    /// </summary>
    public void InitializeDB() {

        DalTest.Initialization.DO();
        AdminManager.UpdateClock(AdminManager.Now);
    }

}

    
    