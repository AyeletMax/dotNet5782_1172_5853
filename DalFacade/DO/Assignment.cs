namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id">unique Id for call</param>
/// <param name="VolunteerId">id of the volunteer accepting the call</param>
/// <param name="EntranceTime">time and date that the call began getting taken care of</param>
/// <param name="ExitTime">time and date the volunteer finished taking care of the call</param>
/// <param name="FinishCallType">the way the call ended</param>
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime EntranceTime,
    DateTime? ExitTime=null,
    FinishCallType? FinishCallType = null
    
)
{
   
    /// <summary>
    ///  Default constructor
    /// </summary>
    public Assignment() : this(0, 0, 0, DateTime.MinValue, DateTime.MinValue, default(FinishCallType)) { }

}


