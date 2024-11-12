namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="ID">unique Id for call</param>
/// <param name="CallId">id for the call the volunteer chose to accept</param>
/// <param name="VolunteerId">id of the volunteer accepting the call</param>
/// <param name="EntranceTime">time and date that the call began getting taken care of</param>
/// <param name="ExitTime">time and date the volunteer finished taking care of the call</param>
/// <param name="FinishCallType">the way the call ended</param>
public record Assignment
(


    int ID,
    int CallId,
    int VolunteerId,
    DateTime EntranceTime,
    DateTime ExitTime,
    FinishCallType FinishCallType

)
{

}
