namespace DO;
/// <summary>
/// Volunteer Entity represents a volunteer with all its props
/// </summary>
/// <param name="Id">Personal unique ID of the volunteer</param>
/// <param name="FirstName">Private name of the volunteer</param>
/// <param name="LastName">Volunteers Last Name</param>
/// <param name="Phone">Volunteer phone number</param>
/// <param name="Email">Volunteer email address</param>
/// <param name="Password">Volunteer password</param>
/// <param name="Address">Volunteer address</param>
/// <param name="Latitude">A number indicating how far a point on Earth is south or north of the equator.</param>
/// <param name="Longitude">A number indicating how far a point on Earth is east or west of the equator.</param>
/// <param name="Active">Indicates if the volunteer is active or not</param>
/// <param name="LargestDistance">The largest distance for accepting a call</param>
public record Volunteer
(

    int Id,
    string FirstName,
    string LastName,
    string Phone,
    string Email,
    bool Active,
    string? Password = null,
    string? Address = null,
    double? Latitude = null,
    double? Longitude = null,
    Role? MyRole = null,
    double? LargestDistance = null,
    DistanceType MyDistanceType = DistanceType.Air
)
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Volunteer() : this(0, "", "", "", "", true, "", "", 0, 0, Role.Volunteer, 0, DistanceType.Air) { }
}



