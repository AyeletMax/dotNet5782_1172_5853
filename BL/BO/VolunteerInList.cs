
using Helpers;

namespace BO;

public class VolunteerInList
{
    public int Id { get; init; }
    public string Name { get; init; }
    public bool Active { get; init; }
    public int TotalResponsesHandled { get; init; }
    public int TotalResponsesCancelled { get; init; }
    public int TotalExpiredResponses { get; init; }
    public int? AssignedResponseId { get; init; }
    public CallType MyCallType {  get; init; }
    public override string ToString() => this.ToStringProperty();
}



