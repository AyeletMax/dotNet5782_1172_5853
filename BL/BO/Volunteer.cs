
namespace BO;

public class Volunteer
{
    public int Id { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public bool Active { get; set; }
    public Role MyRole { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? LargestDistance { get; set; }
    public DistanceType MyDistanceType { get; set; }
    public int TotalCallsHandled { get; set; }
    public int TotalCallsCancelled { get; set; }
    public int TotalExpiredCallsChosen { get; set; }
    public BO.CallInProgress? CurrentCallInProgress { get; set; }

}

