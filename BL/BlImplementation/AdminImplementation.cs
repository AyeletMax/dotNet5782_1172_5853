using BlApi;
using BO;
using DalApi;
using Helpers;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public DateTime GetClock()
    {
        ClockManager.UpdateClock(ClockManager.Now.AddMinutes(1));
        return ClockManager.Now;
    }
    public void AdvanceClock(TimeUnit unit)
    {
        DateTime newClock = unit switch
        {
            TimeUnit.MINUTE => ClockManager.Now.AddMinutes(1),
            TimeUnit.HOUR => ClockManager.Now.AddHours(1),
            TimeUnit.DAY => ClockManager.Now.AddDays(1),
            TimeUnit.MONTH => ClockManager.Now.AddMonths(1),
            TimeUnit.YEAR => ClockManager.Now.AddYears(1),
            _ => throw new ArgumentOutOfRangeException(nameof(unit), "Invalid time unit")
        };

        ClockManager.UpdateClock(newClock);
    }

    public int GetMaxRange()
    {
        return (int)_dal.Config.RiskRange.TotalMinutes;
    }

    public void SetMaxRange(int maxRange)
    {
        TimeSpan timeSpan = TimeSpan.FromMinutes(maxRange);
        _dal.Config.RiskRange = timeSpan;
    }

    public void ResetDB()
    {
        _dal.Config.Reset(); 
    }
    public void InitializeDB() {

        DalTest.Initialization.DO();
        ClockManager.UpdateClock(ClockManager.Now);
    }

}

    
    