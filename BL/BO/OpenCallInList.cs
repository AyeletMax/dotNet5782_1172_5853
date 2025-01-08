

using Helpers;

namespace BO;

public class OpenCallInList
{
    public int Id { get; init; }
    public CallType MyCallType { get; set; }
    public string? VerbalDescription { get; set; }
    public string Address { get; set; }
    public DateTime OpenTime { get; set; }
   public DateTime? MaxFinishTime { get; set; }
    public double distanceFromVolunteerToCall {  get; set; }
    public override string ToString() => this.ToStringProperty();
}
