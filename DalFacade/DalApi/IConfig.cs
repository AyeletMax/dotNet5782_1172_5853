namespace DalApi;
/// <summary>
/// Interface for system configuration settings.
/// </summary>
public interface IConfig
{
    public TimeSpan RiskRange { get; set; }  // Risk range duration.
    DateTime Clock { get; set; }      // Current system time.
    void Reset();                     // Resets configuration to defaults.
    int NextAssignmentId { get; }     // Gets the next Assignment ID.
    int NextCallId { get; }           // Gets the next Call ID.
}
