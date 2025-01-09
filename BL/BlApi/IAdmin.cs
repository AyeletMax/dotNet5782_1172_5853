
namespace BlApi;

public interface IAdmin
{
    DateTime GetClock();
    void AdvanceClock(BO.TimeUnit unit);
    TimeSpan GetRiskTimeSpan();
    void SetRiskTimeSpan(TimeSpan riskTimeSpan);
    void ResetDatabase();
    void InitializeDatabase();
}
