
namespace BlApi;
/// <summary>
/// The ICall interface defines the operations available for managing calls.
/// It includes methods for retrieving call details, updating call information, adding and deleting calls, and handling various call statuses.
/// </summary>
public interface ICall
{
    /// <summary>
    /// Retrieves the call quantities grouped by their status.
    /// </summary>
    /// <returns>An array of integers representing call quantities by their status.</returns>
    public int[] GetCallQuantitiesByStatus();

    /// <summary>
    /// Retrieves a list of calls, with optional filters and sorting applied.
    /// </summary>
    /// <param name="filterField">An optional field to filter the calls by.</param>
    /// <param name="filterValue">An optional value to apply the filter.</param>
    /// <param name="sortField">An optional field to sort the calls by.</param>
    /// <returns>An enumerable collection of calls based on the provided filter and sort criteria.</returns>
    public IEnumerable<BO.CallInList> GetCallList(
        BO.CallInListFields? filterField = null,
        object? filterValue = null,
        BO.CallInListFields? sortField = null
    );
    /// <summary>
    /// Retrieves details of a specific call.
    /// </summary>
    /// <param name="callId">The unique identifier of the call.</param>
    /// <returns>The call details.</returns>
    public BO.Call GetCallDetails(int callId);

    /// <summary>
    /// Updates an existing call's details.
    /// </summary>
    /// <param name="call">The updated call object.</param>
    public void UpdateCallDetails(BO.Call call);

    /// <summary>
    /// Deletes a call by its unique identifier.
    /// </summary>
    /// <param name="callId">The unique identifier of the call.</param>
    public void DeleteCall(int callId);

    /// <summary>
    /// Adds a new call.
    /// </summary>
    /// <param name="call">The call object to add.</param>
    public void AddCall(BO.Call call);

    /// <summary>
    /// Retrieves a list of closed calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The unique identifier of the volunteer.</param>
    /// <param name="callStatus">An optional call type filter.</param>
    /// <param name="sortField">An optional field to sort the results.</param>
    /// <returns>A collection of closed calls.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(
        int volunteerId,
        BO.CallType? callStatus = null,
        BO.ClosedCallInListFields? sortField = null
    );

    /// <summary>
    /// Retrieves a list of open calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The unique identifier of the volunteer.</param>
    /// <param name="callType">An optional call type filter.</param>
    /// <param name="sortField">An optional field to sort the results.</param>
    /// <returns>A collection of open calls.</returns>
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(
        int volunteerId,
        BO.CallType? callType = null,
        BO.OpenCallInListFields? sortField = null
    );


    /// <summary>
    /// Marks a call as completed by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer completing the call.</param>
    /// <param name="assignmentId">The assignment ID related to the call.</param>
    public void UpdateCallCompletion(
        int volunteerId,
        int assignmentId
    );


    /// <summary>
    /// Cancels a call assignment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer canceling the call.</param>
    /// <param name="assignmentId">The assignment ID related to the call.</param>
    public void UpdateCallCancellation(
        int volunteerId,
        int assignmentId
    );

    /// <summary>
    /// Assigns a call to a volunteer for treatment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer taking the call.</param>
    /// <param name="callId">The ID of the call being assigned.</param>
    public void SelectCallForTreatment(
        int volunteerId,
        int callId
    );

}
