namespace BlApi;
/// <summary>
/// The IBl interface defines the core operations available for business logic.
/// It provides access to the main components for managing volunteers, calls, and administrators.
/// </summary>
public interface IBl
{
    /// <summary>
    /// Gets the Volunteer business logic interface.
    /// Provides access to operations related to managing volunteers.
    /// </summary>
    IVolunteer Volunteer { get; }
    /// <summary>
    /// Gets the Call business logic interface.
    /// Provides access to operations related to managing calls.
    /// </summary>
    ICall Call { get; }
    /// <summary>
    /// Gets the Admin business logic interface.
    /// Provides access to operations related to managing administrative tasks.
    /// </summary>
    IAdmin Admin { get; }
}
