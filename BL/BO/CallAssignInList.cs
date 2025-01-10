

using Helpers;

namespace BO;

public class CallAssignInList
{
    public int? VolunteerId {  get; set; }
    public string? Name { get; set; }
    public DateTime EntranceTime {  get; set; }
    public DateTime? ExitTime {  get; set; }
    public FinishCallType? FinishCallType {  get; set; }
    public override string ToString() => this.ToStringProperty();
}
