

using Helpers;

namespace BO;
/// <summary>
/// Represents an assignment of a volunteer to a call.
/// </summary>
public class CallAssignInList
{
    /// <summary>
    /// Gets the unique identifier of the assigned volunteer.
    /// </summary>
    public int? VolunteerId {  get; init; }
    /// <summary>
    /// Gets the name of the assigned volunteer.
    /// </summary>
    public string? Name { get; init; }
    /// <summary>
    /// Gets the time when the volunteer entered the assignment.
    /// </summary>
    public DateTime EntranceTime {  get; init; }
    // <summary>
    /// Gets the time when the volunteer exited the assignment.
    /// </summary>
    public DateTime? ExitTime {  get; init; }
    /// <summary>
    /// Gets the type of finish call for this assignment.
    /// </summary>
    public FinishCallType? FinishCallType {  get; init; }
    /// <summary>
    /// Returns a string representation of the call assignment.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}
