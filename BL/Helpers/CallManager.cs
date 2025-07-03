using BlApi;
using BO;
using DalApi;
using DO;
using System.Net.Mail;

namespace Helpers;

internal static class CallManager
{
    /// <summary>
    /// An instance of the data access layer (DAL).
    /// </summary>
    private static IDal s_dal = DalApi.Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 

    internal static Status GetCallStatus(int callId)
    {
        try
        {
            DO.Call? call;
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
            {

                call = s_dal.Call.Read(callId);
                assignments = s_dal.Assignment.ReadAll(a => a.CallId == callId);
            }

            if (!assignments.Any() ||
                assignments.Any(a => (a.ExitTime.HasValue
                && (a.FinishCallType == DO.FinishCallType.CanceledByManager || a.FinishCallType == DO.FinishCallType.CanceledByVolunteer)
                 && !assignments.Any(a => (!a.ExitTime.HasValue && a.FinishCallType == null || a.FinishCallType == DO.FinishCallType.TakenCareOf)))))
            {
              Status myStatus = Tools.CalculateStatus(call!);
                if (myStatus == Status.InProgress)
                    return Status.Opened;
                else if (myStatus == Status.Expired)
                    return Status.Expired;
                else
                    return Status.AtRisk;
            }

            if (assignments.Any(a => a.ExitTime.HasValue && a.FinishCallType == DO.FinishCallType.TakenCareOf))
                return Status.Closed;

            if (call!.MaxFinishTime.HasValue && call.MaxFinishTime.Value < AdminManager.Now)
                return Status.Expired;

            return Tools.CalculateStatus(call);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist", ex);
        }
    }
    /// <summary>
    /// Converts a business object (BO) call to a data object (DO) call.
    /// </summary>
    /// <param name="call">The business object call.</param>
    /// <returns>A data object call.</returns>
    public static DO.Call ConvertBoCallToDoCall(BO.Call call)
    {
        return new DO.Call
        {
            Id = call.Id,
            MyCallType = (DO.CallType)call.MyCallType,
            VerbalDescription = call.VerbalDescription,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpenTime,
            MaxFinishTime = call.MaxFinishTime,
        };
    }

    /// <summary>
    /// Validates the details of a given call.
    /// </summary>
    /// <param name="call">The call to validate.</param>
    /// <exception cref="BlInvalidFormatException">Thrown when call details are invalid.</exception>
    internal static void ValidateCallDetails(BO.Call call)
    {
       
        if (!Enum.IsDefined(typeof(BO.CallType), call.MyCallType))
        {
            throw new BlInvalidFormatException("Invalid call type.");
        }
        if (!string.IsNullOrEmpty(call.VerbalDescription) && call.VerbalDescription.Length > 500)
        {
            throw new BlInvalidFormatException("Description is too long (maximum 500 characters).");
        }
        if (call.CallAssignments != null && call.CallAssignments.Any(a => a.EntranceTime < call.OpenTime))
        {
            throw new BlInvalidFormatException("Assignments cannot start before the call's open time.");
        }
        
        if (call.MaxFinishTime.HasValue && call.MaxFinishTime.Value <= call.OpenTime)
        {
            throw new BO.BlInvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
        }
    }

    /// <summary>
    /// Handles periodic updates for calls based on the clock changes.
    /// </summary>
    /// <param name="oldClock">The previous clock time.</param>
    /// <param name="newClock">The new clock time.</param>
    private static int s_periodicCounter = 0;

   

    internal static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        bool callUpdated = false; //stage 5

        // Check for expired calls
        List<DO.Call> expiredCalls;
        lock (AdminManager.BlMutex)
            expiredCalls = s_dal.Call.ReadAll(c => c.MaxFinishTime < newClock).ToList();

        if (expiredCalls.Count > 0)
            callUpdated = true;

        expiredCalls.ForEach(call =>
        {
            List<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
                assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();

            if (!assignments.Any())
            {
                lock (AdminManager.BlMutex)
                    s_dal.Assignment.Create(new DO.Assignment(
                    CallId: call.Id,
                    VolunteerId: 0,
                    EntranceTime: AdminManager.Now,
                    ExitTime: AdminManager.Now,
                    FinishCallType: (DO.FinishCallType)BO.FinishCallType.Expired
                ));
            }
            List<DO.Assignment> assignmentsWithNull;
            lock (AdminManager.BlMutex)
                assignmentsWithNull = s_dal.Assignment.ReadAll(a => a.CallId == call.Id && a.ExitTime is null).ToList();
            if (assignmentsWithNull.Any())
            {
                assignments.ForEach(assignment =>
                {
                    lock (AdminManager.BlMutex)
                        s_dal.Assignment.Update(assignment with
                        {
                            ExitTime = AdminManager.Now,
                            FinishCallType = (DO.FinishCallType)BO.FinishCallType.Expired
                        });
                    Observers.NotifyItemUpdated(assignment.Id);
                    VolunteerManager.Observers.NotifyItemUpdated(assignment.VolunteerId);

                }
                    );
            }
        });

        // Check for calls that have become at risk (entered risk time range)
        TimeSpan riskRange = s_dal.Config.RiskRange;
        List<DO.Call> atRiskCalls;
        lock (AdminManager.BlMutex)
            atRiskCalls = s_dal.Call.ReadAll(c =>
                c.MaxFinishTime.HasValue &&
                c.MaxFinishTime.Value > newClock &&
                c.MaxFinishTime.Value - newClock <= riskRange).ToList();

        if (atRiskCalls.Count > 0)
            callUpdated = true;
        List<DO.Assignment>? CallWithRiskAsignments;
        var atRiskCallIds = atRiskCalls.Select(c => c.Id).ToList();
        lock (AdminManager.BlMutex)
            CallWithRiskAsignments = s_dal.Assignment.ReadAll(a =>
                atRiskCallIds.Any(id => id == a.CallId)
            ).ToList();
        CallWithRiskAsignments.ForEach(ass =>
        {
            VolunteerManager.Observers.NotifyItemUpdated(ass.VolunteerId);
        });


        // Notify observers for calls that have become at risk
        atRiskCalls.ForEach(call =>
        {
            Observers.NotifyItemUpdated(call.Id);
        });

        bool yearChanged = oldClock.Year != newClock.Year; //stage 5
        if (yearChanged || callUpdated)
        { //stage 5
            Observers.NotifyListUpdated();
            VolunteerManager.Observers.NotifyListUpdated();
        }//stage 5
    }
    public static async Task UpdateCallCoordinatesAsync(DO.Call call)
    {
        var coords = await Tools.GetCoordinatesFromAddress(call.Address);
        if (coords is not null)
        {
            var (latitude, longitude) = coords.Value;

            lock (AdminManager.BlMutex)
            {
                //var doCall = s_dal.Call.Read(callId);
                call = call with
                {
                    Latitude = latitude,
                    Longitude = longitude
                };
                s_dal.Call.Update(call);
            }

            Observers.NotifyItemUpdated(call.Id);
            Observers.NotifyListUpdated();
        }
    }
    /// <summary>
    /// Sends an email to a volunteer regarding a canceled assignment.
    /// </summary>
    /// <param name="volunteer">The volunteer to notify.</param>
    /// <param name="assignment">The canceled assignment.</param>
    internal static void SendEmailToVolunteer(DO.Volunteer volunteer, DO.Assignment assignment)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            var call = s_dal.Call.Read(assignment.CallId)!;
            string subject = "הקצאה בוטלה";
            string body = $"Hello {volunteer.Name},\n\n" +
                          $"Your allocation to handle the call {assignment.Id} has been canceled by the manager.\n" +
                          $"Call Details:\n" +
                          $"Call Number: {assignment.CallId}\n" +
                          $"Call Type: {call.MyCallType}\n" +
                          $"Call Address: {call.Address}\n" +
                          $"Openning Time: {call.OpenTime}\n" +
                          $"Verbal Description: {call.VerbalDescription}\n" +
                          $"Call Entrance Time : {assignment.EntranceTime}\n\n" +
                          $"בברכה,\nמערכת ניהול קריאות";
        
        Tools.SendEmail(volunteer.Email, subject, body);
        } 
    }


    /// <summary>
    /// Notifies available volunteers about a new call.
    /// </summary>
    /// <param name="call">The new call to notify about.</param>
    internal static void NotifyVolunteers(BO.Call call)
    {
        lock (AdminManager.BlMutex) //stage 7
        {
            List<DO.Volunteer> volunteers = s_dal.Volunteer.ReadAll().Where(v => v.Active).ToList();
            foreach (var volunteer in volunteers)
            {
                if (volunteer.Latitude.HasValue && volunteer.Longitude.HasValue && volunteer.LargestDistance.HasValue && call.Latitude.HasValue && call.Longitude.HasValue)
                {
                    double distance = Tools.CalculateDistance((BO.DistanceType)volunteer.MyDistanceType, volunteer.Latitude.Value, volunteer.Longitude.Value, call.Latitude.Value, call.Longitude.Value);
                    if (distance <= volunteer.LargestDistance.Value)
                    {
                        string body = $"A new call is available near you: " +
                            $"Call Type: {call.MyCallType}\n" +
                            $"Verbal Description: {call.VerbalDescription}\n" +
                            $"Call Address: {call.Address}\n" +
                            $"Openning Time: {call.OpenTime}\n" +
                            $"Call Status:{call.MyStatus}";


                        Tools.SendEmail(volunteer.Email, "New Volunteer Call", $"A new call is available near you: {call}");
                    }
                }
            }
        }
    }
}