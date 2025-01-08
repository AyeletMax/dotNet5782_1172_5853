

namespace BO;

public class CallInProgress
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallType MyCallType { get; set; }
    public string? VerbalDescription {  get; set; }
    public string Address { get; set; }
    public DateTime OpenTime {  get; set; }
    public DateTime? MaxFinishTime { get; set; }
    public DateTime? EntranceTime { get; set; }
    public double VolunteerResponseDistance { get; set; }
    public Status MyStatus { get; set; }
}

