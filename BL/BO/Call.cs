using Helpers;

namespace BO;

public class Call
{
    public int Id { get; init; }
    public CallType MyCallType { get; set; }
    public string? VerbalDescription { get; set; }
    public string Address { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime? MaxFinishTime { get; set; }
    public Status MyStatus { get; set; }
    public List<BO.CallAssignInList>? callAssignments {  get; set; }
    public override string ToString() => this.ToStringProperty();
}



                  