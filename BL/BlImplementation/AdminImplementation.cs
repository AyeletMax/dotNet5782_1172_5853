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
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
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


    public void InitializeDB()
    {
        //AdminManager.InitializeDB();
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB(); //stage 7

    }
    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        //AdminManager.ResetDB();//Stage 4
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.ResetDB(); //stage 7

    }

    /// <summary>
    /// Returns the maximum risk range in the system.
    /// </summary>
    public TimeSpan GetMaxRange()
    {
        return AdminManager.MaxRange;
    }
    /// <summary>
    /// Sets the maximum risk range in the system.
    /// </summary>
    /// <param name="maxRange">The new risk range to be set.</param>
    public void SetMaxRange(TimeSpan maxRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7

        AdminManager.MaxRange = maxRange;
    }
    /// <summary>
    /// Resets the database and system clock.
    /// </summary>
    /// 
    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator()
    => AdminManager.Stop(); //stage 7
    public void AddClockObserver(Action clockObserver) => AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) => AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) => AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>AdminManager.ConfigUpdatedObservers -= configObserver;
}


