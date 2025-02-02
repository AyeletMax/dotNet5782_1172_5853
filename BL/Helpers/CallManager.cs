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
}


//public static object GetFieldValue(Call call, string fieldName)
//{
//    return fieldName switch
//    {
//        nameof(Call.MyCallType) => call.MyCallType!,
//        nameof(Call.Address) => call.Address!,
//        nameof(Call.Latitude) => call.Latitude!,
//        nameof(Call.Longitude) => call.Longitude!,
//        nameof(Call.OpenTime) => call.OpenTime!,
//        nameof(Call.MaxFinishTime) => call.MaxFinishTime!,
//        nameof(Call.VerbalDescription) => call.VerbalDescription!,
//        nameof(Call.MyStatus) => call.MyStatus!,
//        _ => throw new ArgumentException("Invalid field name", nameof(fieldName))
//    };
//}

//internal static BO.Call ConvertToBOCall(DO.Call call)
//{
//    return new BO.Call
//    {
//        Id = call.Id,
//        Address = call.Address,
//        Latitude = call.Latitude,
//        Longitude = call.Longitude,
//        OpenTime = call.OpenTime,
//        MaxFinishTime = call.MaxFinishTime,
//        VerbalDescription = call.VerbalDescription,
//        // MyStatus = Status.InProgress // או סטטוס אחר אם יש לך לוגיקה אחרת
//    };
//}


//internal static (double? Latitude, double? Longitude) logicalChecking(BO.Call boCall)
//{
//    //if (boCall.MaxFinishTime.HasValue && boCall.MaxFinishTime.Value <= boCall.OpenTime)
//    //{
//    //    throw new BO.BlInvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
//    //}
//    //// Validate that the open time is not in the future
//
//
//    return Tools.GetCoordinatesFromAddress(boCall.Address);
//}