using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Globalization;
using System.Threading.Channels;

namespace BlImplementation;

internal class CallImplementation : BlApi.ICall
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

    public IEnumerable<CallInList> GetCallList(BO.CallSortField? filterField = null, object? filterValue = null, BO.CallSortField? sortField = null)
    {
        try
        {
            // קריאה לכל הקריאות ממסד הנתונים
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
                        MyStatus = CallManager.GetCallStatus(c.Id, _dal),
                        Id = c.Id,
                        OpenTime = c.OpenTime,
                        TimeRemainingToCall = c.MaxFinishTime?.Subtract(_dal.Config.Clock),
                        LastVolunteer = lastAssignment != null ? _dal.Volunteer.Read(lastAssignment.VolunteerId)?.Name : null,
                        CompletionTime = lastAssignment?.ExitTime.HasValue == true
                            ? lastAssignment.ExitTime.Value - lastAssignment.EntranceTime
                            : null,
                    };
                });
            if (filterField.HasValue && filterValue != null)
            {
                calls = calls.Where(c => c.GetType().GetProperty(filterField.ToString())?.GetValue(c)?.Equals(filterValue) == true);
            }

            return sortField.HasValue
                ? calls.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c))
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
            // Create the BO.Call object with the necessary details
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
                MyStatus = Helpers.CallManager.GetCallStatus(callId, _dal),
                callAssignments = callAssignments
            };
     
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
            throw new BO.BlDoesNotExistException($"Call with ID={call.Id} does not exists", ex);
        }
        catch (Exception ex)
        {
            // Catch the data layer exception and rethrow a custom exception to the UI layer
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while update.", ex);
        }
    }

    // Delete a call
    public void DeleteCall(int callId)
    {
        //var call = DataAccess.GetCall(callId);
        var call = _dal.Call.Read(callId); // Fetch the call details

        if (call == null)
        {
            throw new ArgumentException("The call with the specified ID does not exist.");
        }

        // Step 2: Fetch the latest assignment for the call
        var latestAssignment = _dal.Assignment.ReadAll() // Get all assignments
            .Where(a => a.CallId == callId) // Filter by CallId
            .OrderByDescending(a => a.EntranceTime) // Get the latest by EntryTime
            .FirstOrDefault(); // May return null if no assignments exist

        // Step 3: Calculate the status using the helper method
        var status = Helpers.CallManager.GetCallStatus(call.Id, _dal);

        // Step 4: Check if the call can be deleted
        if (status != Status.Opened)
        {
            throw new BO.BlDeletionException("The call cannot be deleted because it is not in an open state.");
        }

        if (latestAssignment != null)
        {
            throw new BO.BlDeletionException("The call cannot be deleted because it has been assigned to a volunteer.");
        }

        // Step 5: Attempt to delete the call
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
                throw new BO.BlInvalidFormatException("The address must be valid and resolvable to latitude and longitude.");
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
            throw new BO.BlAlreadyExistsException("Failed to add the call to the system.");
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while add.", ex);
        }
    }

    //צריך לעדכן אתהENUN שלCallSortField
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callType = null, BO.CallSortField? sortField = null)
    {
        try
        {
            // שלוף את כל ההקצאות של המתנדב
            var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.ExitTime != null)
                .Where(a => callType == null || (BO.CallType)_dal.Call.Read(a.CallId).MyCallType == callType)
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

            // אם נבחר שדה למיון, מיון לפי השדה הנבחר
            return sortField.HasValue
                ? assignments.OrderBy(a => a.GetType().GetProperty(sortField.ToString())?.GetValue(a))
                : assignments.OrderBy(a => a.Id);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the closed calls list.", ex);
        }
    }
      


    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callType = null, BO.CallSortField? sortField = null)
    {
        try
        {
            // שלב 1: קבלת כל הקריאות הפתוחות או הפתוחות בסיכון מה-DAL
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist.");
            var openCalls = _dal.Call.ReadAll()
                .Where(c =>                
                // מחשבים סטטוס של כל קריאה
                (CallManager.GetCallStatus(c.Id, _dal) == BO.Status.Opened || CallManager.GetCallStatus(c.Id, _dal) == BO.Status.AtRisk)) // הפשטת הבדיקה
                .Select(c => new BO.OpenCallInList
                {
                    Id = volunteerId, // ת.ז של המתנדב
                    MyCallType = (BO.CallType)c.MyCallType, // סוג הקריאה
                    VerbalDescription = c.VerbalDescription, // תיאור מילולי
                    Address = c.Address, // כתובת הקריאה
                    OpenTime = c.OpenTime, // זמן פתיחת הקריאה
                    MaxFinishTime = c.MaxFinishTime, // זמן סיום משוער
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
            var assignment = _dal.Assignment.Read(assignmentId);
            if (assignment == null)
            {
                throw new BO.BlDoesNotExistException($"Assignment with ID {assignmentId} not found.");
            }

            // בדיקה אם המתנדב המבצע את הבקשה הוא המתנדב שההקצאה רשומה עליו
            if (assignment.VolunteerId != volunteerId)
            {
                throw new BO.BlUnauthorizedAccessException($"Volunteer with ID {volunteerId} is not authorized to complete this assignment.");
            }

            // בדיקה שההקצאה פתוחה (לא טופלה, לא בוטלה, לא פג תוקף)
            if (assignment.FinishCallType.HasValue)
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
            var status = CallManager.GetCallStatus(assignment.CallId, _dal);

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
            var status = CallManager.GetCallStatus(callId, _dal);

            if (status == Status.Expired || status == Status.Closed || (status == Status.InProgress && _dal.Assignment.Read(callId)!=null))
            {
                throw new BO.BlInvalidOperationException($"Cannot select this call for treatment, since the call's status is: {status}");
            }

            var newAssignment = new DO.Assignment(
                Id: 0,
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
        catch (Exception ex)
        {
            throw new BO.BlInvalidOperationException("An error occurred while selecting the call for treatment.", ex);
        }
    }

}



//// Calculate average handling time for closed calls
//public double CalculateAverageHandlingTime()
//{
//}


//// Retrieve calls by address
//public IEnumerable<CallInList> GetCallsByAddress(string address)
//{
//    var calls = DataAccess.GetCalls().Where(c => c.Address.Contains(address, StringComparison.OrdinalIgnoreCase));
//    return calls.Select(call => new CallInList
//    {
//        CallNumber = call.CallNumber,
//        Status = call.Status,
//        LastAssignment = call.Assignments.OrderByDescending(a => a.AssignmentDate).FirstOrDefault()
//    });
//}




// Retrieve closed calls handled by a specific volunteer
//public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callStatus = null, BO.FinishCallType? sortField = null)
//{
//    try
//    {
//        var closedCalls = _dal.Call.ReadAll()
//            .Where(c => c.Id == volunteerId && CallManager.GetCallStatus(c.Id, _dal) == BO.Status.Closed)
//            .Select(c => new ClosedCallInList
//            {
//                Id = c.Id,
//                CallType = (BO.CallType)c.MyCallType,
//                CallAddress = c.Address,
//                OpenningTime = c.OpenTime,
//                TreatmentStartTime = c.TreatmentStartTime,
//                ActualTreatmentEndTime = c.ActualTreatmentEndTime,
//                FinishCallType = (BO.FinishCallType?)c.FinishCallType
//            });
//        if (callStatus.HasValue)
//        {
//            closedCalls = closedCalls.Where(c => c.CallType == callStatus.Value);
//        }
//        if (sortField.HasValue)
//        {
//            closedCalls = sortField.Value switch
//            {
//                BO.FinishCallType.TakenCareOf => closedCalls.OrderBy(c => c.FinishCallType),
//                BO.FinishCallType.CanceledByVolunteer => closedCalls.OrderBy(c => c.FinishCallType),
//                BO.FinishCallType.CanceledByManager => closedCalls.OrderBy(c => c.FinishCallType),
//                BO.FinishCallType.Expired => closedCalls.OrderBy(c => c.FinishCallType),
//                _ => closedCalls.OrderBy(c => c.Id)
//            };
//        }
//        else
//        {
//            closedCalls = closedCalls.OrderBy(c => c.Id);
//        }

//        return closedCalls!;
//    }
//    catch (Exception ex)
//    {
//        throw new BO.BlGeneralDatabaseException("An error occurred while retrieving the closed calls list.", ex);
//    }
//}


////לבדוק עם מישהי מה יש בRETURN
//public IEnumerable<CallInList> GetCallList(BO.CallSortField? filterField = null, object? filterValue = null, BO.CallSortField? sortField = null)
//{
//    try
//    {
//        var calls = _dal.Call.ReadAll()
//        .Select(c =>
//        {
//            var lastAssignment = _dal.Assignment.ReadAll(a => a.CallId == c.Id)
//                .OrderByDescending(a => a.EntranceTime)
//                .FirstOrDefault();

//            return new BO.CallInList
//            {
//                TotalAllocations = _dal.Assignment.ReadAll(a => a.CallId == c.Id).Count(),
//                CallId = c.Id,
//                CallType = (BO.CallType)c.MyCallType,
//                MyStatus = CallManager.GetCallStatus(c.Id, _dal),
//                Id = lastAssignment?.Id,
//                OpenTime = c.OpenTime,
//                TimeRemainingToCall = c.MaxFinishTime?.Subtract(_dal.Config.Clock),
//                LastVolunteer = lastAssignment != null ? _dal.Volunteer.Read(lastAssignment.VolunteerId)?.Name : null,
//                CompletionTime = lastAssignment?.ExitTime.HasValue == true
//                    ? lastAssignment.ExitTime.Value - lastAssignment.EntranceTime
//                    : null,
//            };
//        });

//        if (filterField.HasValue && filterValue != null)
//        {
//            calls = calls.Where(c => c.GetType().GetProperty(filterField.ToString())?.GetValue(c)?.Equals(filterValue) == true);
//        }

//        return sortField.HasValue
//            ? calls.OrderBy(c => c.GetType().GetProperty(sortField.ToString())?.GetValue(c))
//            : calls.OrderBy(c => c.CallId);
//    }
//    catch (Exception ex)
//    {
//        throw new BO.BlGeneralDatabaseException("Failed to retrieve calls list", ex);
//    }
//}