using BlApi;
using BO;
using DalApi;
using DO;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4
    public static Status GetCallStatus(int callId)
    {
        var call = s_dal.Call.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID {callId} not found.");
        var assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.CallId == callId);
        TimeSpan? timeLeft = call.MaxFinishTime - ClockManager.Now;

        if (call.MaxFinishTime.HasValue && timeLeft < TimeSpan.Zero)
            return Status.Expired;
        if (assignment != null && timeLeft <= s_dal.Config.RiskRange)
            return Status.InProgressAtRisk;
        if (assignment != null)
            return Status.InProgress;
        if (timeLeft <= s_dal.Config.RiskRange)
            return Status.AtRisk;

        return Status.Opened;
    }

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

    internal static void ValidateCallDetails(BO.Call call)
    {
        // Validate that the address is a valid address with latitude and longitude
        if (string.IsNullOrWhiteSpace(call.Address) ||
            !(call.Latitude >= -90 && call.Latitude <= 90 &&
              call.Longitude >= -180 && call.Longitude <= 180))
        {
            throw new BlInvalidFormatException("The address must be valid with latitude and longitude.");
        }
        // Validate the call type is valid
        if (!Enum.IsDefined(typeof(BO.CallType), call.MyCallType))
        {
            throw new BlInvalidFormatException("Invalid call type.");
        }
        // Validate the description length
        if (!string.IsNullOrEmpty(call.VerbalDescription) && call.VerbalDescription.Length > 500)
        {
            throw new BlInvalidFormatException("Description is too long (maximum 500 characters).");
        }
        // Validate that there are no assignments in the past
        if (call.callAssignments != null && call.callAssignments.Any(a => a.EntranceTime < call.OpenTime))
        {
            throw new BlInvalidFormatException("Assignments cannot start before the call's open time.");
        }
        //Validate that the open time is not in the future
        if (call.OpenTime > DateTime.Now)
        {
            throw new BlInvalidFormatException("The open time cannot be in the future.");
        }
        //Validate that the endTime is past the openTime
        if (call.MaxFinishTime.HasValue && call.MaxFinishTime.Value <= call.OpenTime)
        {
            throw new BO.BlInvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
        }
    }
    internal static void PeriodicCallUpdates(DateTime oldClock, DateTime newClock)
    {
        List<DO.Call> expiredCalls = s_dal.Call.ReadAll(c => c.MaxFinishTime > newClock).ToList();

        expiredCalls.ForEach(call =>
        {
            List<DO.Assignment> assignments = s_dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();
            if (!assignments.Any())
                s_dal.Assignment.Create(new DO.Assignment(
                CallId: call.Id,
                VolunteerId: 0,
                EntranceTime: ClockManager.Now,
                ExitTime: ClockManager.Now,
                FinishCallType: (DO.FinishCallType)BO.FinishCallType.Expired
            ));
            List<DO.Assignment> assignmentsWithNull = s_dal.Assignment.ReadAll(a => a.CallId == call.Id && a.FinishCallType is null).ToList();
            if (assignmentsWithNull.Any())
                assignments.ForEach(assignment =>
                    s_dal.Assignment.Update(assignment with
                    {
                        ExitTime = ClockManager.Now,
                        FinishCallType = (DO.FinishCallType)BO.FinishCallType.Expired
                    }));
        });

    }


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

    internal static void NotifyVolunteers(BO.Call call)
    {
        List<DO.Volunteer> volunteers = s_dal.Volunteer.ReadAll().Where(v => v.Active).ToList();
        foreach (var volunteer in volunteers)
        {
            if (volunteer.Latitude.HasValue && volunteer.Longitude.HasValue && volunteer.LargestDistance.HasValue)
            {
                double distance = Tools.CalculateDistance(volunteer.Latitude.Value, volunteer.Longitude.Value, call.Latitude, call.Longitude, (BO.DistanceType)volunteer.MyDistanceType);
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

