

using Helpers;

namespace BO;

public class CallAssignInList
{
    public int? VolunteerId {  get; init; }
    public string? Name { get; init; }
    public DateTime EntranceTime {  get; init; }
    public DateTime? ExitTime {  get; init; }
    public FinishCallType? FinishCallType {  get; init; }
    public override string ToString() => this.ToStringProperty();
}
