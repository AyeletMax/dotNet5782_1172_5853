
using Helpers;

namespace BO;

public class Volunteer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool Active { get; set; }
    public Role MyRole { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? LargestDistance { get; set; }
    public DistanceType MyDistanceType { get; set; } = DistanceType.Air;
    public int TotalCallsHandled { get; init; }
    public int TotalCallsCancelled { get; init; }
    public int TotalExpiredCallsChosen { get; init; }
    public BO.CallInProgress? CurrentCallInProgress { get; init; }
    public override string ToString() => this.ToStringProperty();
}




