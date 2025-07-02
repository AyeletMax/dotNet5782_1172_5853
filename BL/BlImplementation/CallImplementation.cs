using BO;
using DO;
using Helpers;

namespace BlImplementation;
/// <summary>
/// Initializes a new instance of the <see cref="CallImplementation"/> class.
/// </summary>
/// <param name="dataSource">The data source containing call records.</param>
internal class CallImplementation : BlApi.ICall
{

    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    /// <summary>
    /// Retrieves the quantities of calls grouped by their status.
    /// </summary>
    /// <returns>An array of call quantities for each status.</returns>
    public int[] GetCallQuantitiesByStatus()
    {
        try
        {
            var counts = new int[Enum.GetValues(typeof(BO.Status)).Length];
            lock (AdminManager.BlMutex)
            { //stage 7

                _dal.Call.ReadAll()
                .GroupBy(call => (int)CallManager.GetCallStatus(call.Id))
                .ToList()
                .ForEach(g => counts[g.Key] = g.Count());
                return counts;
            }
        }
        catch (Exception ex)
        {
            throw new BlGeneralDatabaseException("Failed to retrieve calls list", ex);
        }
    }
    /// <summary>
    /// Retrieves a list of calls, optionally filtered and sorted by specified fields.
    /// </summary>
    /// <param name="filterField">Optional filter field for filtering the calls.</param>
    /// <param name="filterValue">Optional filter value to filter calls by.</param>
    /// <param name="sortField">Optional field to sort the calls by.</param>
    /// <returns>An enumerable list of filtered and sorted calls.</returns>
    public IEnumerable<CallInList> GetCallList(BO.CallInListFields? filterField = null, object? filterValue = null, BO.CallInListFields? sortField = null)
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                var calls = _dal.Call.ReadAll()
                .Select(c =>
                {
                    var assignments = _dal.Assignment.ReadAll(a => a.CallId == c.Id);
                    var lastAssignment = assignments.OrderByDescending(a => a.EntranceTime).FirstOrDefault();
                    return new BO.CallInList
                    {
                        TotalAllocations = assignments.Count(),
                        CallId = c.Id,
                        CallType = (BO.CallType)c.MyCallType,
                        MyStatus = CallManager.GetCallStatus(c.Id),
                        Id = c.Id,
                        OpenTime = c.OpenTime,
                        TimeRemainingToCall = c.MaxFinishTime?.Subtract(_dal.Config.Clock),
                        LastVolunteer = lastAssignment != null ? _dal.Volunteer.Read(lastAssignment.VolunteerId)?.Name : null,
                        CompletionTime = lastAssignment?.ExitTime.HasValue == true ? lastAssignment.ExitTime.Value - lastAssignment.EntranceTime : null,
                    };
                });
                if (filterField.HasValue && filterValue != null)
                {
                    var prop = typeof(BO.CallInList).GetProperty(filterField.ToString());
                    if (prop != null)
                    {
                        if (prop.PropertyType.IsEnum)
                        {
                            var enumValue = Enum.Parse(prop.PropertyType, filterValue.ToString());
                            calls = calls.Where(c => prop.GetValue(c)?.Equals(enumValue) == true);
                        }
                        else
                        {
                            var convertedValue = Convert.ChangeType(filterValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                            calls = calls.Where(c => prop.GetValue(c)?.Equals(convertedValue) == true);
                        }
                    }
                }

                return sortField.HasValue
                    ? calls.OrderBy(c => typeof(BO.CallInList).GetProperty(sortField.ToString())?.GetValue(c))
                    : calls.OrderBy(c => c.CallId);
            }
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("Failed to retrieve calls list", ex);
        }
    }


    /// <summary>
    /// Retrieves the details of a specific call based on its ID.
    /// </summary>
    /// <param name="callId">The ID of the call to retrieve.</param>
    /// <returns>A <see cref="BO.Call"/> object containing the details of the specified call.</returns>
    public BO.Call GetCallDetails(int callId)
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                DO.Call? call = _dal.Call.Read(callId) ?? throw new BlDoesNotExistException($"Call with ID {callId} was not found.");
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

                return new BO.Call
                {
                    Id = call.Id,
                    MyCallType = (BO.CallType)call.MyCallType,
                    VerbalDescription = call.VerbalDescription,
                    Address = call.Address,
                    Latitude = call.Latitude,
                    Longitude = call.Longitude,
                    OpenTime = call.OpenTime,
                    MaxFinishTime = call.MaxFinishTime,
                    MyStatus = CallManager.GetCallStatus(callId),
                    CallAssignments = callAssignments
                };
            }
        }
        catch (BlDoesNotExistException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while fetching call details.", ex);
        }
    }
    /// <summary>
    /// Updates the details of an existing call after validation.
    /// </summary>
    /// <param name="call">The updated call object.</param>
    //public void UpdateCallDetails(BO.Call call)
    //{
    //    try
    //    {
    //        DO.Call callToUpdate;
    //        AdminManager.ThrowOnSimulatorIsRunning();//stage 7
    //        lock (AdminManager.BlMutex) //stage 7
    //        {
    //            var existingCall = _dal.Call.Read(call.Id) ?? throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does not exist");
    //            CallManager.ValidateCallDetails(call);
    //            if (call.Address != "No Address")
    //            {
    //                var (latitude, longitude) = Tools.GetCoordinatesFromAddress(call.Address!);
    //                call.Latitude = latitude;
    //                call.Longitude = longitude;
    //            }
    //            else
    //            {
    //                call.Address = existingCall.Address;
    //                call.Latitude = existingCall.Latitude;
    //                call.Longitude = existingCall.Longitude;
    //            }
    //            callToUpdate = CallManager.ConvertBoCallToDoCall(call);
    //            _dal.Call.Update(callToUpdate);
    //        }
    //        CallManager.NotifyVolunteers(call);
    //        CallManager.Observers.NotifyItemUpdated(callToUpdate.Id); // Stage 5
    //        CallManager.Observers.NotifyListUpdated(); // Stage 5
    //    }
    //    catch (DO.DalDoesNotExistException ex)
    //    {
    //        throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does not exists", ex);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlGeneralDatabaseException("An unexpected error occurred while update.", ex);
    //    }
    //}
    public void UpdateCallDetails(BO.Call call)
    {
        try
        {
            DO.Call callToUpdate;
            AdminManager.ThrowOnSimulatorIsRunning();//stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                var existingCall = _dal.Call.Read(call.Id) ?? throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does not exist");
                CallManager.ValidateCallDetails(call);

                if (call.Address != "No Address")
                {
                    call.Latitude = null;
                    call.Longitude = null;
                }
                else
                {
                    call.Address = existingCall.Address;
                    call.Latitude = existingCall.Latitude;
                    call.Longitude = existingCall.Longitude;
                }

                callToUpdate = CallManager.ConvertBoCallToDoCall(call);
                _dal.Call.Update(callToUpdate);
            }
            CallManager.Observers.NotifyItemUpdated(callToUpdate.Id);
            CallManager.Observers.NotifyListUpdated();

            if (call.Address != "No Address")
                _ = CallManager.UpdateCallCoordinatesAsync(callToUpdate);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does not exists", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while update.", ex);
        }
    }

    /// <summary>
    /// Deletes a call from the system by its ID.
    /// </summary>
    /// <param name="callId">The ID of the call to delete.</param>
    public void DeleteCall(int callId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();//stage 7
            lock (AdminManager.BlMutex)
            { //stage 7
                var call = _dal.Call.Read(callId) ?? throw new BlDoesNotExistException("The call with the specified ID does not exist.");

                var status = CallManager.GetCallStatus(call.Id);

                if ((status == Status.Opened && status == Status.InProgressAtRisk) || _dal.Assignment.ReadAll(a => a.CallId == callId).Any())
                {
                    throw new BO.BlDeletionException($"The call with ID:{callId} cannot be deleted.");
                }
                _dal.Call.Delete(callId); 
            }
                CallManager.Observers.NotifyListUpdated(); // Stage 5
        }
        catch (BlDeletionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while deleting.", ex);
        }
    }
    /// <summary>
    /// Adds a new call to the system after validating its details.
    /// </summary>
    /// <param name="call">The call object to add.</param>
    //public void AddCall(BO.Call call)
    //{
    //    try
    //    {
    //        AdminManager.ThrowOnSimulatorIsRunning();//stage 7
    //        CallManager.ValidateCallDetails(call);
    //        var (latitude, longitude) = Tools.GetCoordinatesFromAddress(call.Address);
    //        call.Latitude = latitude;
    //        call.Longitude = longitude;
    //        DO.Call dataCall = CallManager.ConvertBoCallToDoCall(call);
    //        lock (AdminManager.BlMutex) //stage 7
    //            _dal.Call.Create(dataCall);
    //        CallManager.NotifyVolunteers(call);
    //        CallManager.Observers.NotifyListUpdated(); // Stage 5
    //    }
    //    catch (BlInvalidFormatException)
    //    {
    //        throw;
    //    }
    //    catch (DO.DalAlreadyExistsException)
    //    {
    //        throw new BO.BlAlreadyExistsException("Failed to add the call to the system.");
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlGeneralDatabaseException("An unexpected error occurred while add.", ex);
    //    }
    //}
    public void AddCall(BO.Call call)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();//stage 7
            CallManager.ValidateCallDetails(call);

            call.Latitude = null;
            call.Longitude = null;

            DO.Call dataCall = CallManager.ConvertBoCallToDoCall(call);
            lock (AdminManager.BlMutex) //stage 7
                _dal.Call.Create(dataCall);
            
            CallManager.Observers.NotifyListUpdated();
            DO.Call call1 = _dal.Call.Read(c => (dataCall.Address == c.Address &&
            dataCall.VerbalDescription == c.VerbalDescription &&
            dataCall.MaxFinishTime == c.MaxFinishTime &&
            dataCall.OpenTime == c.OpenTime &&
            dataCall.Latitude == c.Latitude &&
            dataCall.Longitude == c.Longitude && dataCall.MyCallType == c.MyCallType))!;
            CallManager.NotifyVolunteers(call);
            if (call1.Address != "No Address")
                _ = CallManager.UpdateCallCoordinatesAsync(call1);
         

        }
        catch (BlInvalidFormatException)
        {
            throw;
        }
        catch (DO.DalAlreadyExistsException)
        {
            throw new BO.BlAlreadyExistsException("Failed to add the call to the system.");
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while add.", ex);
        }
    }

    /// <summary>
    /// Retrieves the list of closed calls for a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The volunteer ID to filter closed calls by.</param>
    /// <param name="callTypeFilter">Optional filter for call type.</param>
    /// <param name="sortField">Optional field to sort the closed calls by.</param>
    /// <returns>An enumerable list of closed calls for the volunteer.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callTypeFilter = null, BO.ClosedCallInListFields? sortField = null)
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.ExitTime != null)
                .Where(a => callTypeFilter is null || (BO.CallType)_dal.Call.Read(a.CallId).MyCallType == callTypeFilter)
                .Select(a =>
                {
                    var call = _dal.Call.Read(a.CallId);
                    return new BO.ClosedCallInList
                    {
                        Id = call.Id,
                        CallType = (BO.CallType)call.MyCallType,
                        CallAddress = call.Address,
                        OpeningTime = call.OpenTime,
                        TreatmentStartTime = a.EntranceTime,
                        ActualTreatmentEndTime = a.ExitTime,
                        FinishCallType = (BO.FinishCallType)a.FinishCallType
                    };
                });

                return sortField.HasValue
                    ? assignments.OrderBy(a => a.GetType().GetProperty(sortField.ToString())?.GetValue(a))
                    : assignments.OrderBy(a => a.Id);
            }
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the closed calls list.", ex);
        }
    }
    /// <summary>
    /// Retrieves a list of open calls assigned to a specific volunteer, with optional filters and sorting.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="callType">Optional filter to return only specific call types.</param>
    /// <param name="sortField">Optional field to sort the list of open calls.</param>
    /// <returns>A sorted list of open calls assigned to the volunteer.</returns>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="BO.BlGeneralDatabaseException">Thrown if an error occurs while retrieving the list.</exception>
    //public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callType = null, BO.OpenCallInListFields? sortField = null)
    //{
    //    try
    //    {
    //        var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
    //        var openCalls = _dal.Call.ReadAll()
    //            .Where(c => (CallManager.GetCallStatus(c.Id) == BO.Status.Opened || CallManager.GetCallStatus(c.Id) == BO.Status.AtRisk))
    //            .Select(c => new BO.OpenCallInList
    //            {
    //                Id = c.Id,
    //                MyCallType = (BO.CallType)c.MyCallType,
    //                VerbalDescription = c.VerbalDescription,
    //                Address = c.Address,
    //                OpenTime = c.OpenTime,
    //                MaxFinishTime = c.MaxFinishTime,
    //                distanceFromVolunteerToCall = Tools.CalculateDistance((BO.DistanceType)volunteer.MyDistanceType, volunteer.Latitude ?? double.MaxValue, volunteer.Longitude ?? double.MaxValue, c.Latitude, c.Longitude),
    //            });
    //        return sortField.HasValue
    //        ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c))
    //        : openCalls.OrderBy(c => c.Id);

    //    }
    //    catch (Exception ex)
    //    {
    //        throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the open calls list.", ex);
    //    }
    //}
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callType = null, BO.OpenCallInListFields? sortField = null)
    {
        try
        {
            lock (AdminManager.BlMutex)
            { //stage 7

                var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
                var openCalls = _dal.Call.ReadAll()
                    .Where(c => (CallManager.GetCallStatus(c.Id) == BO.Status.Opened || CallManager.GetCallStatus(c.Id) == BO.Status.AtRisk))
                    .Where(c => callType == null || (BO.CallType)c.MyCallType == callType) // הוספת סינון לפי callType
                    .Select(c => new BO.OpenCallInList
                    {
                        Id = c.Id,
                        MyCallType = (BO.CallType)c.MyCallType,
                        VerbalDescription = c.VerbalDescription,
                        Address = c.Address,
                        OpenTime = c.OpenTime,
                        MaxFinishTime = c.MaxFinishTime,
                        distanceFromVolunteerToCall = Tools.CalculateDistance((BO.DistanceType)volunteer.MyDistanceType, volunteer.Latitude ?? double.MaxValue, volunteer.Longitude ?? double.MaxValue, c.Latitude, c.Longitude),
                    });

                return sortField switch
                {
                    OpenCallInListFields.Id => openCalls.OrderBy(c => c.Id),
                    OpenCallInListFields.CallType => openCalls.OrderBy(c => c.MyCallType),
                    OpenCallInListFields.FullAddress => openCalls.OrderBy(c => c.Address),
                    OpenCallInListFields.Start_time => openCalls.OrderBy(c => c.OpenTime),
                    OpenCallInListFields.Max_finish_time => openCalls.OrderBy(c => c.MaxFinishTime),
                    OpenCallInListFields.CallDistance => openCalls.OrderBy(c => c.distanceFromVolunteerToCall),
                    OpenCallInListFields.Verbal_description => openCalls.OrderBy(c => c.VerbalDescription),
                    _ => openCalls.OrderBy(c => c.Id)
                };
            }
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the open calls list.", ex);
        }
    }
    /// <summary>
    /// Updates the completion status of an assignment for a volunteer, marking it as completed.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer completing the assignment.</param>
    /// <param name="assignmentId">The ID of the assignment being completed.</param>
    /// <exception cref="BO.BlDoesNotExistException">Thrown if the assignment does not exist.</exception>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the volunteer is not authorized to complete the assignment.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the assignment is already completed or canceled.</exception>
    /// <exception cref="BO.BlGeneralDatabaseException">Thrown if an error occurs while updating the assignment completion.</exception>
    public void UpdateCallCompletion(int volunteerId, int assignmentId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();//stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                var assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} not found.");

                if (assignment.VolunteerId != volunteerId)
                {
                    throw new BO.BlUnauthorizedAccessException($"Volunteer with ID {volunteerId} is not authorized to complete this assignment.");
                }

                if (assignment.FinishCallType != null || assignment.ExitTime != null)
                {
                    throw new BO.BlInvalidOperationException("The assignment has already been completed or canceled.");
                }

                var updatedAssignment = assignment with
                {
                    FinishCallType = (DO.FinishCallType?)BO.FinishCallType.TakenCareOf,
                    ExitTime = AdminManager.Now
                };
                    _dal.Assignment.Update(updatedAssignment);
                CallManager.Observers.NotifyItemUpdated(updatedAssignment.CallId); // Stage 5
                CallManager.Observers.NotifyListUpdated();
                VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
                VolunteerManager.Observers.NotifyListUpdated(); // Stage 5
            }
        }
        catch (BlUnauthorizedAccessException)
        {
            throw;
        }
        catch (BlInvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while updating the assignment completion.", ex);
        }
    }
    /// <summary>
    /// Updates the cancellation status of an assignment for a volunteer, marking it as canceled.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer canceling the assignment.</param>
    /// <param name="assignmentId">The ID of the assignment being canceled.</param>
    /// <exception cref="ArgumentException">Thrown if the assignment or volunteer cannot be found.</exception>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the volunteer does not have permission to cancel the assignment.</exception>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the assignment is already completed or in a non-cancellable state.</exception>
    /// <exception cref="BO.BlGeneralDatabaseException">Thrown if an error occurs while updating the call cancellation.</exception>
    public void UpdateCallCancellation(int volunteerId, int assignmentId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();//stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                var assignment = _dal.Assignment.Read(assignmentId) ?? throw new KeyNotFoundException($"Assignment with ID {assignmentId} not found.");
                var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new KeyNotFoundException($"Volunteer with ID {volunteerId} not found.");
                if (volunteer.MyRole != DO.Role.Manager && assignment.VolunteerId != volunteerId)
                {
                    throw new BO.BlUnauthorizedAccessException("You do not have permission to cancel this assignment.");
                }

                var status = CallManager.GetCallStatus(assignment.CallId);

                if (status == Status.Expired || status == Status.Closed)
                {
                    throw new BO.BlInvalidOperationException($"Cannot cancel an assignment that is {status}.");
                }

                assignment = assignment with
                {
                    ExitTime = AdminManager.Now,
                    FinishCallType = (assignment.VolunteerId == volunteerId) ? DO.FinishCallType.CanceledByVolunteer : DO.FinishCallType.CanceledByManager
                };
                CallManager.SendEmailToVolunteer(volunteer, assignment);
                    _dal.Assignment.Update(assignment);
                CallManager.Observers.NotifyItemUpdated(assignment.CallId); // Stage 5
                CallManager.Observers.NotifyListUpdated();
                VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
                VolunteerManager.Observers.NotifyListUpdated();// Stage 5
            }
        }
        catch (KeyNotFoundException ex)
        {
            throw new ArgumentException(ex.Message, ex);
        }
        catch (BO.BlUnauthorizedAccessException ex)
        {
            throw new BO.BlInvalidOperationException(ex.Message, ex);
        }
        catch (BlInvalidOperationException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidOperationException("An error occurred while updating the call cancellation.", ex);
        }
    }

    /// <summary>
    /// Assigns a volunteer to a call for treatment, marking the call as being handled by the volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer selecting the call for treatment.</param>
    /// <param name="callId">The ID of the call to be selected for treatment.</param>
    /// <exception cref="BO.BlInvalidOperationException">Thrown if the call is already closed, expired, or in progress.</exception>
    /// <exception cref="BO.BlGeneralDatabaseException">Thrown if an error occurs while selecting the call for treatment.</exception>
    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();//stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                var call = _dal.Call.Read(callId) ?? throw new BO.BlInvalidOperationException($"Call with ID {callId} not found.");
                var status = CallManager.GetCallStatus(callId);

                if (status == Status.Expired || status == Status.Closed || (status == Status.InProgress && _dal.Assignment.Read(callId) != null))
                {
                    throw new BO.BlInvalidOperationException($"Cannot select this call for treatment, since the call's status is: {status}");
                }

                var newAssignment = new DO.Assignment(
                    CallId: callId,
                    VolunteerId: volunteerId,
                    EntranceTime: AdminManager.Now,
                    ExitTime: null,
                    FinishCallType: null
                );
                    _dal.Assignment.Create(newAssignment);
                CallManager.Observers.NotifyListUpdated(); // Stage 5
            }
        }
        catch (BO.BlInvalidOperationException ex)
        {
            throw new BO.BlInvalidOperationException($"Invalid operation: {ex.Message}", ex);
        }
    }
    #region Stage 5
    public void AddObserver(Action listObserver) =>
            CallManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
            CallManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
            CallManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
            CallManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}