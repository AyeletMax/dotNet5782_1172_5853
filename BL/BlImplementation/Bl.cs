namespace BlImplementation;
using BlApi;
/// <summary>
/// The Bl class is the entry point for accessing the Business Logic (BL) layer of the application. 
/// It exposes implementations for handling Volunteers, Calls, and Admin functionalities.
/// </summary>
internal class Bl : IBl
{
    /// <summary>
    /// Gets the instance of the Volunteer implementation.
    /// </summary>
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    /// <summary>
    /// Gets the instance of the Call implementation.
    /// </summary>
    public ICall Call { get; } = new CallImplementation();
    /// <summary>
    /// Gets the instance of the Admin implementation.
    /// </summary>
    public IAdmin Admin { get; } = new AdminImplementation();
}
