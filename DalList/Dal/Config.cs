using System.Runtime.CompilerServices;

namespace Dal;
/// <summary>
/// Configuration Entity
/// </summary>
/// <param name="nextCallId">an ID number for the next new call</param>
/// <param name="nextAssignmentId">a unique id for each new assignment</param>
/// <param name="Clock">a system clock that is maintained seperatly from the pc</param>
/// <param name="RiskRange">a time range from which there and on it is considered at risk</param>


internal static class Config 
{
    internal const int StartAssignmentId = 1;
    private static int nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId{ get => nextAssignmentId++; }

    internal const int StartCallId = 1;
    private static int nextCallId = StartCallId;
    internal static int NextCallId { get => nextCallId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;
    
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Reset()
    {
        nextAssignmentId = StartAssignmentId;
        nextCallId = StartCallId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
