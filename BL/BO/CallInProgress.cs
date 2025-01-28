

using Helpers;

namespace BO;

public class CallInProgress
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallType MyCallType { get; init; }
    public string? VerbalDescription {  get; init; }
    public string Address { get; init; }
    public DateTime OpenTime {  get; init; }
    public DateTime? MaxFinishTime { get; init; }
    public DateTime? EntranceTime { get; init; }
    public double VolunteerResponseDistance { get; init; }
    public Status MyStatus { get; init; }
    public override string ToString() => this.ToStringProperty();
}

