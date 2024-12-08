using DalApi;

namespace Dal;

/// <summary>
/// Provides an implementation of the IConfig interface.
/// Manages configuration settings and system-wide properties.
/// </summary>
internal class ConfigImplementation : IConfig
{
    /// Gets or sets the system clock used for tracking current time.
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// Resets all configuration settings to their default values.
    public void Reset()
    {
        Config.Reset();
    }
    /// Gets the next available ID for assignments.
    public int NextAssignmentId
    {
        get => Config.NextAssignmentId;
    }
    /// Gets the next available ID for calls.
    public int NextCallId
    {
        get => Config.NextCallId;
    }
    /// Gets or sets the time span defining the risk range.
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
}
