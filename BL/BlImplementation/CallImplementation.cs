using BlApi;
using BO;
using Helpers;

namespace BlImplementation;

internal class CallImplementation:ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public int[] GetCallQuantitiesByStatus()
    {
        var calls = _dal.Call.ReadAll();
        return calls.GroupBy(call => CallManager.GetCallStatus(call.Id, _dal))
                    .OrderBy(g => g.Key)
                    .Select(g => g.Count())
                    .ToArray();
    }

    public IEnumerable<CallInList> GetCallList(CallType? filterField = null, object? filterValue = null, Status? sortField = null)
    {
        var calls = _dal.Call.ReadAll();
        if (filterField.HasValue && filterValue != null)
        {
            calls = from call in calls
                    let boCall = CallManager.ConvertToBOCall(call)
                    where CallManager.GetFieldValue(boCall, filterField.Value.ToString())?.Equals(filterValue) ?? false
                    select call;
        }
        if (sortField.HasValue)
        {
            calls = from call in calls
                    let boCall = CallManager.ConvertToBOCall(call)
                    orderby CallManager.GetFieldValue(boCall, sortField.Value.ToString())
                    select call;
        }
        else
        {
            calls = calls.OrderBy(call => call.Id);
        }
        return calls.Select(call =>

        {
            var boCall = CallManager.ConvertToBOCall(call);
            return new CallInList
            {
                CallId = boCall.Id,
                CallType = boCall.MyCallType,
                MyStatus = boCall.MyStatus,
                LastVolunteer = boCall.callAssignments?.OrderByDescending(a => a.EntranceTime).FirstOrDefault()?.Name,
            };
        });
    }

    // Retrieve details of a specific call
    public Call GetCallDetails(int callId)
    {
        var call = _dal.Call.Read(callId);
        if (call == null) throw new Exception("Call not found.");

        return new Call
        {
            CallNumber = call.CallNumber,
            Description = call.Description,
            Status = call.Status,
            Assignments = call.Assignments.Select(a => new CallAssignInList
            {
                AssignmentId = a.AssignmentId,
                VolunteerId = a.VolunteerId,
                AssignmentDate = a.AssignmentDate
            }).ToList()
        };
    }

    // Update call details
    public void UpdateCallDetails(Call call)
    {
        if (string.IsNullOrWhiteSpace(call.Address))
            throw new Exception("Invalid address.");

        if (call.EndTime < call.StartTime)
            throw new Exception("End time must be after start time.");

        var doCall = new Call
        {
            CallNumber = call.CallNumber,
            Description = call.Description,
            Address = call.Address,
            Status = call.Status
        };

        _dal.Call.Update(doCall);
    }

    // Delete a call
    public void DeleteCall(int callId)
    {
        var call = DataAccess.GetCall(callId);
        if (call == null) throw new Exception("Call not found.");

        if (call.Status != Status.Open || call.Assignments.Any())
            throw new Exception("Cannot delete a call that is not open or has assignments.");

        _dal.Call.Delete(callId);
    }

    // Add a new call
    public void AddCall(Call call)
    {
        if (string.IsNullOrWhiteSpace(call.Address))
            throw new Exception("Invalid address.");

        var doCall = new Call
        {
            CallNumber = call.CallNumber,
            Description = call.Description,
            Address = call.Address,
            Status = call.Status
        };

        DataAccess.AddCall(doCall);
    }

    // Retrieve closed calls handled by a specific volunteer
    public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallType? callStatus = null, FinishCallType? sortField = null)
    {
        var calls = DataAccess.GetCalls()
                              .Where(c => c.Status == Status.Closed && c.Assignments.Any(a => a.VolunteerId == volunteerId));

        if (callStatus.HasValue)
        {
            calls = calls.Where(c => c.CallType == callStatus.Value);
        }

        calls = sortField.HasValue ? calls.OrderBy(c => c.GetFieldValue(sortField.Value)) : calls.OrderBy(c => c.CallNumber);

        return calls.Select(call => new ClosedCallInList
        {
            CallNumber = call.CallNumber,
            FinishType = call.FinishType,
            CloseDate = call.CloseDate
        });
    }

    // Calculate average handling time for closed calls
    public double CalculateAverageHandlingTime()
    {
        var calls = DataAccess.GetCalls().Where(c => c.Status == Status.Closed);
        return !calls.Any() ? 0 : calls.Average(call => (call.CloseDate - call.StartDate).TotalMinutes);
    }

    // Retrieve calls by address
    public IEnumerable<CallInList> GetCallsByAddress(string address)
    {
        var calls = DataAccess.GetCalls().Where(c => c.Address.Contains(address, StringComparison.OrdinalIgnoreCase));
        return calls.Select(call => new CallInList
        {
            CallNumber = call.CallNumber,
            Status = call.Status,
            LastAssignment = call.Assignments.OrderByDescending(a => a.AssignmentDate).FirstOrDefault()
        });
    }

    // Check the status of a specific call
    public Status CheckCallStatus(int callId)
    {
        var call = DataAccess.GetCall(callId);
        if (call == null) throw new Exception("Call not found.");

        return call.Status;
    }

    public IEnumerable<CallInProgress> GetOpenCallsForVolunteer(int volunteerId, CallType? callType = null, Status? sortField = null)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallCompletion(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallCancellation(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }
}
