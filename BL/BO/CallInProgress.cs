

using Helpers;

namespace BO;
/// <summary>
/// Represents an ongoing call in progress with relevant details.
/// </summary>
public class CallInProgress
{
    /// <summary>
    /// Gets the unique identifier of the call in progress.
    /// </summary>
    public int Id { get; init; }

    // <summary>
    /// Gets the unique identifier of the original call.
    /// </summary>
    public int CallId { get; init; }

    /// <summary>
    /// Gets the type of the call in progress.
    /// </summary>
    public CallType MyCallType { get; init; }

    /// <summary>
    /// Gets a verbal description of the call in progress.
    /// </summary>
    public string? VerbalDescription {  get; init; }


    /// <summary>
    /// Gets the address of the call location.
    /// </summary>
    public string Address { get; init; }

    /// <summary>
    /// Gets the time when the call was opened.
    /// </summary>
    public DateTime OpenTime {  get; init; }

    /// <summary>
    /// Gets the maximum allowed finish time for the call.
    /// </summary>
    public DateTime? MaxFinishTime { get; init; }

    /// <summary>
    /// Gets the time when the volunteer entered the call.
    /// </summary>
    public DateTime? EntranceTime { get; init; }

    /// <summary>
    /// Gets the distance the volunteer responded from.
    /// </summary>
    public double VolunteerResponseDistance { get; init; }

    /// <summary>
    /// Gets the current status of the call in progress.
    /// </summary>
    public Status MyStatus { get; init; }

    /// <summary>
    /// Returns a string representation of the call in progress.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}

