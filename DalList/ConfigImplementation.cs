using DalApi;

namespace Dal;

public class ConfigImplementation : IConfig
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
    public TimeSpan RiskRange
    {
        get=> Config.RiskRange;
        set => Config.RiskRange = value;
    }
}
