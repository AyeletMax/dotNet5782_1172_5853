
using Helpers;

namespace BO;

public class VolunteerInList
{
    /// The unique identifier of the volunteer.
    public int Id { get; init; }
    /// The name of the volunteer.
    public string Name { get; init; }
    /// Indicates whether the volunteer is active.
    public bool Active { get; init; }
    /// The total number of responses handled by the volunteer.
    public int TotalResponsesHandled { get; init; }
    /// The total number of responses canceled by the volunteer.
    public int TotalResponsesCancelled { get; init; }
    /// The total number of responses that expired.
    public int TotalExpiredResponses { get; init; }
    /// The ID of the response currently assigned to the volunteer (if any).
    public int? AssignedResponseId { get; init; }
    /// The type of call the volunteer handles.
    public CallType MyCallType {  get; init; }
    /// Returns a string representation of the object.
    public override string ToString() => this.ToStringProperty();
}



