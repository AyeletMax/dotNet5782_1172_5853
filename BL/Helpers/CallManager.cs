

using BO;
using DalApi;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    public static Status GetCallStatus(int callId, IDal dal)
    {
        var call = dal.Call.Read(callId);

        if (call == null)
            throw new KeyNotFoundException($"Call with ID {callId} not found.");

        if (call.MaxFinishTime.HasValue && call.MaxFinishTime.Value < DateTime.Now)
            return Status.Expired;

        if ((DateTime.Now - call.OpenTime).TotalHours > 1)
            return Status.AtRisk;

        return Status.InProgress;
    }


    public static object GetFieldValue(Call call, string fieldName)
    {
        return fieldName switch
        {
            nameof(Call.MyCallType) => call.MyCallType!,
            nameof(Call.Address) => call.Address!,
            nameof(Call.Latitude) => call.Latitude!,
            nameof(Call.Longitude) => call.Longitude!,
            nameof(Call.OpenTime) => call.OpenTime!,
            nameof(Call.MaxFinishTime) => call.MaxFinishTime!,
            nameof(Call.VerbalDescription) => call.VerbalDescription!,
            nameof(Call.MyStatus) => call.MyStatus!,
            _ => throw new ArgumentException("Invalid field name", nameof(fieldName))
        };
    }

    internal static BO.Call ConvertToBOCall(DO.Call call)
    {
        return new BO.Call
        {
            Id = call.Id,
            Address = call.Address,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            OpenTime = call.OpenTime,
            MaxFinishTime = call.MaxFinishTime,
            VerbalDescription = call.VerbalDescription,
           // MyStatus = Status.InProgress // או סטטוס אחר אם יש לך לוגיקה אחרת
        };
    }


    internal static void ValidateCallDetails(BO.Call call)
    {
        // Validate that the address is a valid address with latitude and longitude
        if (string.IsNullOrWhiteSpace(call.Address) ||
            !(call.Latitude >= -90 && call.Latitude <= 90 &&
              call.Longitude >= -180 && call.Longitude <= 180))
        {
            throw new ArgumentException("The address must be valid with latitude and longitude.");
        }
        // Validate the call type is valid
        if (!Enum.IsDefined(typeof(BO.CallType), call.MyCallType))
        {
            throw new ArgumentException("Invalid call type.");
        }
        // Validate the description length
        if (!string.IsNullOrEmpty(call.VerbalDescription) && call.VerbalDescription.Length > 500)
        {
            throw new ArgumentException("Description is too long (maximum 500 characters).");
        }
        // Validate that there are no assignments in the past
        if (call.callAssignments != null && call.callAssignments.Any(a => a.EntranceTime < call.OpenTime))
        {
            throw new ArgumentException("Assignments cannot start before the call's open time.");
        }
    }

    internal static (double? Latitude, double? Longitude) logicalChecking(BO.Call boCall)
    {
        if (boCall.MaxFinishTime.HasValue && boCall.MaxFinishTime.Value <= boCall.OpenTime)
        {
            throw new BO.InvalidOperationException("The MaxEndTime must be greater than the OpenTime.");
        }
        // Validate that the open time is not in the future
        if (boCall.OpenTime > DateTime.Now)
        {
            throw new ArgumentException("The open time cannot be in the future.");
        }

        return Tools.GetCoordinatesFromAddress(boCall.Address);


    }
}
