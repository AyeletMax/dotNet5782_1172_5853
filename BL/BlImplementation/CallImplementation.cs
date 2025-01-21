using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;

namespace BlImplementation;

internal class CallImplementation :ICall
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

    // לבדוק עם מישהי מה יש בRETURN
    public IEnumerable<CallInList> GetCallList(BO.CallType? filterField = null, object? filterValue = null, Status? sortField = null)
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

    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            DO.Call? call = _dal.Call.Read(callId);
            // If no call was found, throw an exception
            if (call == null)
            {
                throw new Exception($"Call with ID {callId} was not found.");
            }
            var callAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId)
                                             .Select(a => new BO.CallAssignInList
                                             {
                                                 VolunteerId = a.VolunteerId,
                                                 Name = _dal.Volunteer.Read(a.VolunteerId)?.Name,
                                                 EntranceTime = a.EntranceTime,
                                                 ExitTime = a.ExitTime,
                                                 FinishCallType = (BO.FinishCallType?)a.FinishCallType
                                             })
                                             .ToList();
            // Create the BO.Call object with the necessary details
            BO.Call callInList = new BO.Call
            {
                Id = call.Id,
                MyCallType = (BO.CallType)call.MyCallType,
                VerbalDescription = call.VerbalDescription,
                Address = call.Address,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                OpenTime = call.OpenTime,
                MaxFinishTime = call.MaxFinishTime,
                MyStatus = Helpers.CallManager.GetCallStatus(callId, _dal),
                callAssignments = callAssignments
            };
            return callInList;
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while fetching call details.", ex);
        }
    }

    // Update call details
    public void UpdateCallDetails(BO.Call call)
    {
        try
        {
            Helpers.CallManager.ValidateCallDetails(call);
            var (latitude, longitude) = Helpers.CallManager.logicalChecking(call);

            if (latitude is null || longitude is null)
            {
                throw new ArgumentException("The address must be valid and resolvable to latitude and longitude.");
            }

            // Update the properties of the updatedCall instance
            call.Latitude = latitude.Value;
            call.Longitude = longitude.Value;


            // Convert BO.Call to DO.Call for data layer update
            DO.Call callToUpdate = new DO.Call
            {
                Id = call.Id,
                MyCallType = (DO.CallType)call.MyCallType,
                VerbalDescription = call.VerbalDescription,
                Address = call.Address,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                OpenTime = call.OpenTime,
                MaxFinishTime = call.MaxFinishTime
            };

            // Attempt to update the call in the data layer

            _dal.Call.Update(callToUpdate);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.DalDoesNotExistException($"Call with ID={call.Id} does not exists");
        }
        catch (Exception ex)
        {
            // Catch the data layer exception and rethrow a custom exception to the UI layer
            throw new BO.GeneralDatabaseException("An unexpected error occurred while update.", ex);
        }
    }

    // Delete a call
    public void DeleteCall(int callId)
    {
        //var call = DataAccess.GetCall(callId);
        var call = _dal.Call.Read(callId);

        if (call == null) throw new Exception("Call not found.");

        if (call.Status != Status.Open || call.Assignments.Any())
            throw new Exception("Cannot delete a call that is not open or has assignments.");

        _dal.Call.Delete(callId);
    }

    public void AddCall(BO.Call call)
    {
        try
        {
            Helpers.CallManager.ValidateCallDetails(call);
            var (latitude, longitude) = Helpers.CallManager.logicalChecking(call);
            if (latitude is null || longitude is null)
            {
                throw new ArgumentException("The address must be valid and resolvable to latitude and longitude.");
            }
            call.Latitude = latitude.Value;
            call.Longitude = longitude.Value;
            var dataCall = new DO.Call
            {
                MyCallType = (DO.CallType)call.MyCallType,
                VerbalDescription = call.VerbalDescription,
                Address = call.Address,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                OpenTime = call.OpenTime,
                MaxFinishTime = call.MaxFinishTime
            };
            _dal.Call.Create(dataCall);
        }
        catch (DO.DalAlreadyExistsException)
        {
            throw new BO.DalAlreadyExistsException("Failed to add the call to the system.");
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while add.", ex);
        }
    }

    // Retrieve closed calls handled by a specific volunteer
    public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallType? callStatus = null, FinishCallType? sortField = null)
    {
        var calls = _dal.Call.ReadAll ()

        //var calls = DataAccess.GetCalls()
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
