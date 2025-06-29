using DalApi;
using System.Runtime.CompilerServices;

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
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.Clock;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.Clock = value;
    }

    /// Resets all configuration settings to their default values.
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Reset()
    {
        Config.Reset();
    }
    /// Gets the next available ID for assignments.
    public int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.NextAssignmentId;
    }
    /// Gets the next available ID for calls.
    public int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.NextCallId;
    }
    /// Gets or sets the time span defining the risk range.
    public TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get => Config.RiskRange;
        [MethodImpl(MethodImplOptions.Synchronized)]
        set => Config.RiskRange = value;
    }
}
