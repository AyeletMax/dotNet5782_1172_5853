
using Helpers;

namespace BO;

public class VolunteerInList
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool Active { get; set; }
    public int TotalResponsesHandled { get; set; }
    public int TotalResponsesCancelled { get; set; }
    public int TotalExpiredResponses { get; set; }
    public int? AssignedResponseId { get; set; }
    public CallType MyCallType {  get; set; }
    public override string ToString() => this.ToStringProperty();
}
