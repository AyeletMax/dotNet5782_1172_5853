
namespace BlApi;

public interface IAdmin
{
    DateTime GetClock();
    void InitializeDB();
    void ResetDB();
    int GetMaxRange();
    void SetMaxRange(int maxRange);
    void AdvanceClock(BO.TimeUnit unit);
}
