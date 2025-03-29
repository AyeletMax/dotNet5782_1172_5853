using Helpers;

namespace BO;

/// <summary>
/// Represents a closed call entry with details about its resolution.
/// </summary>
public class ClosedCallInList
{
    /// <summary>
    /// Gets the unique identifier of the closed call.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Gets the type of the closed call.
    /// </summary>
    public CallType CallType { get; init; }

    /// <summary>
    /// Gets the address where the call took place.
    /// </summary>
    public string CallAddress { get; init; }

    /// <summary>
    /// Gets the time when the call was opened.
    /// </summary>
    public DateTime OpeningTime { get; init; }

    /// <summary>
    /// Gets the time when treatment started.
    /// </summary>
    public DateTime TreatmentStartTime { get; init; }

    /// <summary>
    /// Gets the actual time when treatment ended, if available.
    /// </summary>
    public DateTime? ActualTreatmentEndTime { get; init; }

    /// <summary>
    /// Gets the type of finish for the closed call.
    /// </summary>
    public FinishCallType? FinishCallType { get; init; }

    /// <summary>
    /// Returns a string representation of the closed call entry.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}
