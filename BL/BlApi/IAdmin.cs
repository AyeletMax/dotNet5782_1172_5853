using BO;
namespace BlApi;

public interface IAdmin
{
    DateTime GetClock();
    void AdvanceClock(TimeUnit unit);
    TimeSpan GetRiskTimeSpan();
    void SetRiskTimeSpan(TimeSpan riskTimeSpan);
    void ResetDatabase();
    void InitializeDatabase();
}
