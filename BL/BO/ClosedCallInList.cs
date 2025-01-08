
using Helpers;

namespace BO;

public class ClosedCallInList
{
    public int Id { get; set; }
    public CallType CallType { get; init; }
    public string CallAddress { get; init; }
    public DateTime OpenningTime { get; init; }
    public DateTime TreatmentStartTime { get; init; }
    public DateTime? ActualTreatmentEndTime { get; init; }
    public FinishCallType? FinishCallType { get; init; }
    public override string ToString() => this.ToStringProperty();

}
