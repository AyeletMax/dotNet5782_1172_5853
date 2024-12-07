using DalApi;

namespace Dal;

internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public void Reset()
    {
        Config.Reset();
    }
    public int NextAssignmentId { get => Config.NextAssignmentId; }
    public int NextCallId { get => Config.NextCallId; }
    public TimeSpan RiskRange
    {
        get=> Config.RiskRange;
        set => Config.RiskRange = value;
    }
}
