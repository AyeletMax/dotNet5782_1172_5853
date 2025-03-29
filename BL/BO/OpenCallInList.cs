

using Helpers;

namespace BO;
/// <summary>
/// Represents an open call entry in the system.
/// </summary>
public class OpenCallInList
{
    /// <summary>
    /// The unique identifier of the call.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The type of the call.
    /// </summary>
    public CallType MyCallType { get; set; }

    /// <summary>
    /// A verbal description of the call.
    /// </summary>
    public string? VerbalDescription { get; set; }

    /// <summary>
    /// The address where the call is located.
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// The time when the call was opened.
    /// </summary>
    public DateTime OpenTime { get; set; }

    /// <summary>
    /// The maximum allowed finish time for the call.
    /// </summary>
    public DateTime? MaxFinishTime { get; set; }

    /// <summary>
    /// The distance from the volunteer to the call location.
    /// </summary>
    public double distanceFromVolunteerToCall { get; set; }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}
