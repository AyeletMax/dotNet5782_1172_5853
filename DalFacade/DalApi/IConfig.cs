namespace DalApi;

public interface IConfig
{
    public TimeSpan RiskRange { get; set; }
    DateTime Clock { get; set; }
    void Reset();
    //int NextAssignmentId { get; }
    //int NextCallId { get; }
}
