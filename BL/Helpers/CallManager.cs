

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
            nameof(Call.MyCallType) => call.MyCallType,
            nameof(Call.Address) => call.Address,
            nameof(Call.Latitude) => call.Latitude,
            nameof(Call.Longitude) => call.Longitude,
            nameof(Call.OpenTime) => call.OpenTime,
            nameof(Call.MaxFinishTime) => call.MaxFinishTime,
            nameof(Call.VerbalDescription) => call.VerbalDescription,
            nameof(Call.MyStatus) => call.MyStatus,_
                 => throw new ArgumentException("Invalid field name", nameof(fieldName))
        };
    }

    // פונקציה להמיר DO.Call ל-BO.Call
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
            MyStatus = Status.InProgress // או סטטוס אחר אם יש לך לוגיקה אחרת
        };
    }

}
