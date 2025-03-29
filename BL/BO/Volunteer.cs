
using Helpers;

namespace BO;
/// <summary>
/// Represents a volunteer in the system.
/// </summary>
public class Volunteer
{
    /// <summary>
    /// The unique identifier of the volunteer.
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// The name of the volunteer.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The phone number of the volunteer.
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// The email address of the volunteer.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Indicates whether the volunteer is active.
    /// </summary>
    public bool Active { get; set; }

    /// <summary>
    /// The role of the volunteer.
    /// </summary>
    public Role MyRole { get; set; }

    /// <summary>
    /// The password of the volunteer (optional).
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// The address of the volunteer (optional).
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// The latitude of the volunteer's location (optional).
    /// </summary>
    public double? Latitude { get; set; }

    /// <summary>
    /// The longitude of the volunteer's location (optional).
    /// </summary>
    public double? Longitude { get; set; }

    /// <summary>
    /// The maximum distance the volunteer is willing to travel, whether by walking, driving, or air distance.
    /// </summary>
    public double? LargestDistance { get; set; }

    /// <summary>
    /// The type of distance measurement used.
    /// </summary>
    public DistanceType MyDistanceType { get; set; } = DistanceType.Air;

    /// <summary>
    /// The total number of calls the volunteer has handled.
    /// </summary>
    public int TotalCallsHandled { get; init; }

    /// <summary>
    /// The total number of calls the volunteer has canceled.
    /// </summary>
    public int TotalCallsCancelled { get; init; }

    /// <summary>
    /// The total number of expired calls the volunteer has chosen.
    /// </summary>
    public int TotalExpiredCallsChosen { get; init; }


    /// <summary>
    /// The current call in progress for the volunteer (if any).
    /// </summary>
    public BO.CallInProgress? CurrentCallInProgress { get; init; }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    public override string ToString() => this.ToStringProperty();
}




