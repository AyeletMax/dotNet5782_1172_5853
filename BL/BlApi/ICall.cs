
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
    public IEnumerable<BO.CallInList> GetCallList(
        BO.CallInListFields? filterField = null,
        object? filterValue = null,
        BO.CallInListFields? sortField = null
    );
    /// <summary>
    /// Retrieves a list of calls, with optional filters and sorting applied.
    /// </summary>
    /// <param name="filterField">An optional field to filter the calls by.</param>
    /// <param name="filterValue">An optional value to apply the filter.</param>
    /// <param name="sortField">An optional field to sort the calls by.</param>
    /// <returns>An enumerable collection of calls based on the provided filter and sort criteria.</returns>
    public BO.Call GetCallDetails(int callId);

    public void UpdateCallDetails(BO.Call call);
 
    public void DeleteCall(int callId);

    public void AddCall(BO.Call call);

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(
        int volunteerId,
        BO.CallType? callStatus = null,
        BO.ClosedCallInListFields? sortField = null
    );

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(
        int volunteerId,
        BO.CallType? callType = null,
        BO.OpenCallInListFields? sortField = null
    );

    public void UpdateCallCompletion(
        int volunteerId,
        int assignmentId
    );

    public void UpdateCallCancellation(
        int volunteerId,
        int assignmentId
    );

    public void SelectCallForTreatment(
        int volunteerId,
        int callId
    );

}
