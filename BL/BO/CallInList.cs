

using Helpers;

namespace BO;
/// <summary>
/// Represents a summarized call entry with essential details.
/// </summary>
public class CallInList
{
    /// <summary>
    /// Gets the total number of volunteer allocations for this call.
    /// </summary>
    public int TotalAllocations { get; init; }

    /// <summary>
    /// Gets the unique identifier of the call.
    /// </summary>
    public int CallId {  get; init; }

    /// <summary>
    /// Gets the type of the call.
    /// </summary>
    public CallType CallType { get; init; }

    /// <summary>
    /// Gets the current status of the call.
    /// </summary>
    public Status MyStatus { get; init; }

    /// <summary>
    /// Gets the optional identifier of the call entry.
    /// </summary>
    public int? Id { get; init; }

    /// <summary>
    /// Gets the time when the call was opened.
    /// </summary>
    public DateTime? OpenTime { get; init; }

    /// <summary>
    /// Gets the remaining time until the call is scheduled to end.
    /// </summary>
    public TimeSpan? TimeRemainingToCall { get; init; }

    /// <summary>
    /// Gets the name of the last assigned volunteer.
    /// </summary>
    public string? LastVolunteer { get; init; }


    /// <summary>
    /// Gets the total completion time for the call.
    /// </summary>
    public TimeSpan? CompletionTime { get; init; }

    /// <summary>
    /// Returns a string representation of the summarized call entry.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}

