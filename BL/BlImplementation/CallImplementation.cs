using BO;
using Helpers;

namespace BlImplementation;

internal class CallImplementation : BlApi.ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public int[] GetCallQuantitiesByStatus()
    {
        try
        {
            var counts = new int[Enum.GetValues(typeof(BO.Status)).Length];
            _dal.Call.ReadAll()
                .GroupBy(call => (int)CallManager.GetCallStatus(call.Id))
                .ToList()
                .ForEach(g => counts[g.Key] = g.Count());
            return counts;
        }
        catch (Exception ex) 
        { 
            throw new BlGeneralDatabaseException("Failed to retrieve calls list", ex);
        }
    }
    public IEnumerable<CallInList> GetCallList(BO.CallInListFields? filterField = null, object? filterValue = null, BO.CallInListFields? sortField = null)
    {
        try
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
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("Failed to retrieve calls list", ex);
        }
    }



    public BO.Call GetCallDetails(int callId)
    {
        try
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
                callAssignments = callAssignments
            };
     
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

    public void UpdateCallDetails(BO.Call call)
    {
        try
        {
            var existingCall = _dal.Call.Read(call.Id) ?? throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does not exist");
            CallManager.ValidateCallDetails(call);
            if (call.Address != "No Address")
            {
                var (latitude, longitude) = Tools.GetCoordinatesFromAddress(call.Address!);
                if (latitude is null || longitude is null)
                    throw new BO.BlInvalidFormatException($"Invalid address: {call.Address}");
                call.Latitude = latitude.Value;
                call.Longitude = longitude.Value;
            }
            else
            {
                call.Address = existingCall.Address;
                call.Latitude = existingCall.Latitude;
                call.Longitude = existingCall.Longitude;
            }
            DO.Call callToUpdate=CallManager.ConvertBoCallToDoCall(call);
            _dal.Call.Update(callToUpdate);
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

    // Delete a call
    public void DeleteCall(int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId) ?? throw new BlDoesNotExistException("The call with the specified ID does not exist.");

            // Step 3: Calculate the status using the helper method
            var status = CallManager.GetCallStatus(call.Id);

            // Step 4: Check if the call can be deleted
            if ((status == Status.Opened && status == Status.InProgressAtRisk)|| _dal.Assignment.ReadAll(a => a.CallId == callId).Any())
            {
                throw new BO.BlDeletionException($"The call with ID:{callId} cannot be deleted.");
            }                

            // Step 5: Attempt to delete the call
            _dal.Call.Delete(callId);
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

    public void AddCall(BO.Call call)
    {
        try
        {
            CallManager.ValidateCallDetails(call);
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(call.Address);
            if (latitude is null || longitude is null)
            {
                throw new BO.BlInvalidFormatException("The address must be valid and resolvable to latitude and longitude.");
            }
            call.Latitude = latitude.Value;
            call.Longitude = longitude.Value;
            DO.Call dataCall = CallManager.ConvertBoCallToDoCall(call);
            _dal.Call.Create(dataCall);
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

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callTypeFilter = null, BO.ClosedCallInListFields? sortField = null)
    {
        try
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
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the closed calls list.", ex);
        }
    }
     
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callType = null, BO.OpenCallInListFields? sortField = null)
    {
        try
        {
            // שלב 1: קבלת כל הקריאות הפתוחות או הפתוחות בסיכון מה-DAL
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
            var openCalls = _dal.Call.ReadAll()
                .Where(c =>                
                // מחשבים סטטוס של כל קריאה
                (CallManager.GetCallStatus(c.Id) == BO.Status.Opened || CallManager.GetCallStatus(c.Id) == BO.Status.AtRisk)) // הפשטת הבדיקה
                .Select(c => new BO.OpenCallInList
                {
                    Id = volunteerId, 
                    MyCallType = (BO.CallType)c.MyCallType,  
                    VerbalDescription = c.VerbalDescription, 
                    Address = c.Address, 
                    OpenTime = c.OpenTime,   
                    MaxFinishTime = c.MaxFinishTime, 
                    distanceFromVolunteerToCall = Tools.CalculateDistance(volunteer.Latitude,volunteer.Longitude, c.Latitude,c.Longitude)
                });

            // שלב 4: סינון לפי סוג הקריאה (אם לא null)
            return sortField.HasValue
            ? openCalls.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c))
            : openCalls.OrderBy(c => c.Id);

        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the open calls list.", ex);
        }
    }
    
    public void UpdateCallCompletion(int volunteerId, int assignmentId)
    {
        try
        {
            // שליפת ההקצאה מתוך מאגר הנתונים
            var assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} not found.");

            // בדיקה אם המתנדב המבצע את הבקשה הוא המתנדב שההקצאה רשומה עליו
            if (assignment.VolunteerId != volunteerId)
            {
                throw new BO.BlUnauthorizedAccessException($"Volunteer with ID {volunteerId} is not authorized to complete this assignment.");
            }

            // בדיקה שההקצאה פתוחה (לא טופלה, לא בוטלה, לא פג תוקף)
            if (assignment.FinishCallType!=null|| assignment.ExitTime!=null)
            {
                throw new BO.BlInvalidOperationException("The assignment has already been completed or canceled.");
            }

            // יצירת אובייקט חדש עם הערך המעודכן
            var updatedAssignment = assignment with
            {
                FinishCallType = (DO.FinishCallType?)BO.FinishCallType.TakenCareOf, // ערך הסיום
                ExitTime = ClockManager.Now // זמן סיום
            };

            // שמירת השינויים
            _dal.Assignment.Update(updatedAssignment);
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

    public void UpdateCallCancellation(int volunteerId, int assignmentId)
    {
        try
        {
            // 1. פנייה לשכבת הנתונים להבאת ההקצאה לפי מזהה ההקצאה
            var assignment = _dal.Assignment.Read(assignmentId) ?? throw new KeyNotFoundException($"Assignment with ID {assignmentId} not found.");

            // 3. בדיקת הרשאה לביטול (האם המבקש הוא מנהל או המתנדב עצמו)
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new KeyNotFoundException($"Volunteer with ID {volunteerId} not found.");
            if (volunteer.MyRole!= DO.Role.Manager || assignment.VolunteerId != volunteerId)
            {
                throw new BO.BlUnauthorizedAccessException("You do not have permission to cancel this assignment.");
            }

            // 4. בדיקת סטטוס ההקצאה: לוודא שהיא פתוחה ולא טופלה
            var status = CallManager.GetCallStatus(assignment.CallId);

            if (status == Status.Expired || status == Status.Closed)
            {
                throw new BO.BlInvalidOperationException($"Cannot cancel an assignment that is {status}.");
            }

            // 5. עדכון הנתונים של ההקצאה עם יצירת אובייקט חדש בעזרת 'with'
            assignment = assignment with
            {
                ExitTime = ClockManager.Now, // עדכון ExitTime
                FinishCallType = (assignment.VolunteerId == volunteerId) ? DO.FinishCallType.CanceledByVolunteer : DO.FinishCallType.CanceledByManager
            };

            // 7. עדכון ההקצאה בשכבת הנתונים
            _dal.Assignment.Update(assignment);
        }
        catch (KeyNotFoundException ex)
        {
            throw new ArgumentException(ex.Message, ex);
        }
        catch (BO.BlUnauthorizedAccessException ex)
        {
            throw new BO.BlInvalidOperationException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlInvalidOperationException("An error occurred while updating the call cancellation.", ex);
        }
    }


    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        try
        {
            var call = _dal.Call.Read(callId) ?? throw new BO.BlInvalidOperationException($"Call with ID {callId} not found.");
            var status = CallManager.GetCallStatus(callId);

            if (status == Status.Expired || status == Status.Closed || (status == Status.InProgress && _dal.Assignment.Read(callId)!=null))
            {
                throw new BO.BlInvalidOperationException($"Cannot select this call for treatment, since the call's status is: {status}");
            }

            var newAssignment = new DO.Assignment(
                CallId: callId,
                VolunteerId: volunteerId,
                EntranceTime: ClockManager.Now,
                ExitTime: null,
                FinishCallType: null
            );
            _dal.Assignment.Create(newAssignment);
        }
        catch (BO.BlInvalidOperationException ex)
        {
            throw new BO.BlInvalidOperationException($"Invalid operation: {ex.Message}", ex);
        }
    }

}