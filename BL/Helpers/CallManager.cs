using BlApi;
using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class CallManager
{
    /// <summary>
    /// An instance of the data access layer (DAL).
    /// </summary>
    private static IDal s_dal = DalApi.Factory.Get; //stage 4
    internal static ObserverManager Observers = new(); //stage 5 

    /// <summary>
    /// Calculates the status of a call.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>The current status of the call.</returns>
    public static Status GetCallStatus(int callId)
    {
        var call = s_dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID {callId} not found.");
    //    var activeAssignments = s_dal.Assignment.ReadAll()
    //.Where(a => a.CallId == callId && a.ExitTime == null)
    //.ToList();
        var assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.CallId == callId);
        TimeSpan? timeLeft = call.MaxFinishTime - AdminManager.Now;

        if (call.MaxFinishTime.HasValue && timeLeft < TimeSpan.Zero)
            return Status.Expired;
        if (assignment != null && timeLeft <= s_dal.Config.RiskRange)
            return Status.InProgressAtRisk;
        if (assignment != null)
            return Status.InProgress;
        if (timeLeft <= s_dal.Config.RiskRange)
            return Status.AtRisk;

        return Status.Opened;
        //TimeSpan? timeLeft = call.MaxFinishTime - AdminManager.Now;

        //if (call.MaxFinishTime.HasValue && timeLeft < TimeSpan.Zero)
        //    return Status.Expired;

        //if (!activeAssignments.Any())
        //{
        //    if (timeLeft <= s_dal.Config.RiskRange)
        //        return Status.AtRisk;

        //    return Status.Opened;
        //}

        //if (timeLeft <= s_dal.Config.RiskRange)
        //    return Status.InProgressAtRisk;

        //return Status.InProgress;
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
        if (string.IsNullOrWhiteSpace(call.Address) ||
            !(call.Latitude >= -90 && call.Latitude <= 90 &&
              call.Longitude >= -180 && call.Longitude <= 180))
        {
            throw new BlInvalidFormatException("The address must be valid with latitude and longitude.");
        }
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
        if (call.OpenTime > DateTime.Now)
        {
            throw new BlInvalidFormatException("The open time cannot be in the future.");
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
    internal static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        List<DO.Call> expiredCalls = s_dal.Call.ReadAll(c => c.MaxFinishTime > newClock).ToList();

        expiredCalls.ForEach(call =>
        {
            List<DO.Assignment> assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();
            if (!assignments.Any()) { 
                s_dal.Assignment.Create(new DO.Assignment(
                CallId: call.Id,
                VolunteerId: 0,
                EntranceTime: AdminManager.Now,
                ExitTime: AdminManager.Now,
                FinishCallType: (DO.FinishCallType)BO.FinishCallType.Expired
            ));
                Observers.NotifyItemUpdated(call.Id);

            }
            List<DO.Assignment> assignmentsWithNull = s_dal.Assignment.ReadAll(a => a.CallId == call.Id && a.FinishCallType is null).ToList();
            if (assignmentsWithNull.Any())
            {
                assignments.ForEach(assignment =>
                    s_dal.Assignment.Update(assignment with
                    {
                        ExitTime = AdminManager.Now,
                        FinishCallType = (DO.FinishCallType)BO.FinishCallType.Expired
                    }));
                Observers.NotifyItemUpdated(call.Id);
            }
        });

    }

    /// <summary>
    /// Sends an email to a volunteer regarding a canceled assignment.
    /// </summary>
    /// <param name="volunteer">The volunteer to notify.</param>
    /// <param name="assignment">The canceled assignment.</param>
    internal static void SendEmailToVolunteer(DO.Volunteer volunteer, DO.Assignment assignment)
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


    /// <summary>
    /// Notifies available volunteers about a new call.
    /// </summary>
    /// <param name="call">The new call to notify about.</param>
    internal static void NotifyVolunteers(BO.Call call)
    {
        List<DO.Volunteer> volunteers = s_dal.Volunteer.ReadAll().Where(v => v.Active).ToList();
        foreach (var volunteer in volunteers)
        {
            if (volunteer.Latitude.HasValue && volunteer.Longitude.HasValue && volunteer.LargestDistance.HasValue)
            {
                double distance = Tools.CalculateDistance((BO.DistanceType)volunteer.MyDistanceType, volunteer.Latitude.Value, volunteer.Longitude.Value, call.Latitude, call.Longitude);
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