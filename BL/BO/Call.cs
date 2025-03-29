using Helpers;

namespace BO;
/// <summary>
/// Represents a call request with details such as type, address, status, and assignments.
/// </summary>
public class Call
{
    /// <summary>
    /// Gets the unique identifier of the call.
    /// </summary>
    public int Id { get; init; }
    /// <summary>
    /// Gets or sets the type of the call.
    /// </summary>
    public CallType MyCallType { get; set; }
    /// <summary>
    /// Gets or sets a verbal description of the call.
    /// </summary>
    public string? VerbalDescription { get; set; }
    /// <summary>
    /// Gets or sets the address of the call location.
    /// </summary>
    public string Address { get; set; }
    /// <summary>
    /// Gets or sets the latitude coordinate of the call location.
    /// </summary>
    public double Latitude { get; set; }
    /// <summary>
    /// Gets or sets the longitude coordinate of the call location.
    /// </summary>
    public double Longitude { get; set; }
    /// <summary>
    /// Gets or sets the time when the call was opened.
    /// </summary>
    public DateTime OpenTime { get; set; }
    /// <summary>
    /// Gets or sets the maximum allowed finish time for the call.
    /// </summary>
    public DateTime? MaxFinishTime { get; set; }
    /// <summary>
    /// Gets or sets the current status of the call.
    /// </summary>
    public Status MyStatus { get; set; }
    /// <summary>
    /// Gets or sets the list of assignments related to this call.
    /// </summary>
    public List<CallAssignInList>? CallAssignments {  get; set; }
    /// <summary>
    /// Returns a string representation of the call object.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}



