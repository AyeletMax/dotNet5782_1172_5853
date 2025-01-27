using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Threading.Channels;

namespace BlImplementation;

internal class CallImplementation :BlApi.ICall
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
            throw new BO.BLDoesNotExistException($"Call with ID={call.Id} does not exists",ex);
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
            throw new InvalidOperationException("The call cannot be deleted because it is not in an open state.");
        }

        if (latestAssignment != null)
        {
            throw new InvalidOperationException("The call cannot be deleted because it has been assigned to a volunteer.");
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
    public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? callStatus = null, BO.FinishCallType? sortField = null)
    {
        try
        {
            var closedCalls = _dal.Call.ReadAll()
                .Where(c => c.Id == volunteerId && CallManager.GetCallStatus(c.Id, _dal) == BO.Status.Closed)
                .Select(c => new ClosedCallInList
                {
                    Id = c.Id,
                    CallType = (BO.CallType)c.MyCallType,
                    CallAddress = c.Address,
                    OpenningTime = c.OpenTime,
                    TreatmentStartTime = c.TreatmentStartTime, 
                    ActualTreatmentEndTime = c.ActualTreatmentEndTime, 
                    FinishCallType = (BO.FinishCallType?)c.FinishCallType 
                });
            if (callStatus.HasValue)
            {
                closedCalls = closedCalls.Where(c => c.CallType == callStatus.Value);
            }
            if (sortField.HasValue)
            {
                closedCalls = sortField.Value switch
                {
                    BO.FinishCallType.TakenCareOf => closedCalls.OrderBy(c => c.FinishCallType),
                    BO.FinishCallType.CanceledByVolunteer => closedCalls.OrderBy(c => c.FinishCallType),
                    BO.FinishCallType.CanceledByManager => closedCalls.OrderBy(c => c.FinishCallType),
                    BO.FinishCallType.Expired => closedCalls.OrderBy(c => c.FinishCallType),
                    _ => closedCalls.OrderBy(c => c.Id)
                };
            }
            else
            {
                closedCalls = closedCalls.OrderBy(c => c.Id); 
            }

            return closedCalls!;
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An error occurred while retrieving the closed calls list.", ex);
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


    public IEnumerable<CallInProgress> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? callType = null, BO.Status? sortField = null)
    {
        try
        {
            // שלב 1: קבלת כל הקריאות הפתוחות או הפתוחות בסיכון מה-DAL
            var openCalls = _dal.Call.ReadAll()
                .Where(c =>
                {
                    // מחשבים סטטוס של כל קריאה
                    var status = CallManager.GetCallStatus(c.Id, _dal);
                    return status == BO.Status.Opened || status == BO.Status.AtRisk;
                }).Select(c => new CallInProgress
                {
                    Id = volunteerId, // ת.ז של המתנדב
                    CallId = c.Id, // מזהה הקריאה
                    MyCallType = (BO.CallType)c.MyCallType, // סוג הקריאה
                    VerbalDescription = c.VerbalDescription, // תיאור מילולי
                    Address = c.Address, // כתובת הקריאה
                    OpenTime = c.OpenTime, // זמן פתיחת הקריאה
                    MaxFinishTime = c.MaxFinishTime, // זמן סיום משוער
                    EntranceTime = null, // נתון שאינו קיים בשלב זה
                   //MyStatus = (BO.Status)c.MyStatus // סטטוס הקריאה
                    MyStatus = CallManager.GetCallStatus(c.Id, _dal)
                });

            // שלב 2: השגת הקואורדינטות של המתנדב מה-DAL
            var volunteer = _dal.Volunteer.Read(volunteerId); // השגת המתנדב מה-DAL
            if (volunteer == null)
            {
                throw new Exception("Volunteer not found.");
            }

            double volunteerLatitude = volunteer.Latitude ?? 0; 
            double volunteerLongitude = volunteer.Longitude ?? 0;

            // שלב 3: חישוב המרחק בין המתנדב לקריאה לכל קריאה
            foreach (var call in openCalls)
            {
                call.VolunteerResponseDistance = Helpers.Tools.CalculateDistance(
                    volunteerLatitude,
                    volunteerLongitude,
                    call.Latitude,  // קואורדינטות הקריאה (נניח שנמצאות ב-DAL)
                    call.Longitude);
            }

            // שלב 4: סינון לפי סוג הקריאה (אם לא null)
            if (callType.HasValue)
            {
                openCalls = openCalls.Where(c => c.MyCallType == callType.Value);
            }

            // שלב 5: מיון הרשימה
            if (sortField.HasValue)
            {
                openCalls = sortField.Value switch
                {
                    BO.Status.Opened => openCalls.OrderBy(c => c.VolunteerResponseDistance), // מיון לפי מרחק
                    BO.Status.AtRisk => openCalls.OrderBy(c => c.OpenTime), // מיון לפי זמן פתיחה
                    _ => openCalls.OrderBy(c => c.CallId) // ברירת מחדל
                };
            }
            else
            {
                openCalls = openCalls.OrderBy(c => c.CallId); // מיון לפי מזהה הקריאה כברירת מחדל
            }

            return openCalls!;
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An error occurred while retrieving the open calls list.", ex);
        }
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
