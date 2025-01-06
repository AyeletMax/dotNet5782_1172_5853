using DO;

namespace BO;

public class CallInList
{
    public int TotalAllocations { get; set; }
    public int CallId {  get; init; }
    public CallType CallType { get; init; }
    public Status MyStatus { get; init; }
    public int? Id { get; init; }
    public DateTime? OpenTime { get; init; }
    public TimeSpan? TimeRemainingToCall { get; init; }
    public string? LastVolunteer { get; init; }
    public TimeSpan? CompletionTime { get; init; }
}
