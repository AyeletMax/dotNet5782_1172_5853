
namespace BlApi;
// Interface IAdmin defines administrative operations for managing time and database state.

public interface IAdmin
{
    // Returns the current system time (clock).
    // Returns:
    // The current DateTime of the system clock.
    DateTime GetClock();
    // Initializes the database, preparing it for use (e.g., setting up tables, default values).
    void InitializeDB();
    // Resets the database, likely clearing existing data and restoring it to its initial state.
    void ResetDB();

    // Retrieves the maximum allowable time range for a specific operation or system configuration.
    // Returns:
    // The TimeSpan representing the maximum time range.
    TimeSpan GetMaxRange();
    // Sets a new maximum time range for the system or operation.
    // Parameters:
    // maxRange: The new TimeSpan to be set as the maximum range.
    void SetMaxRange(TimeSpan maxRange);
    // Advances the system clock by a specified time unit (e.g., days, months, etc.).
    // Parameters:
    // unit: The time unit (e.g., day, month) by which the clock should be advanced.
    void AdvanceClock(BO.TimeUnit unit);
}
