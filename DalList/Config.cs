namespace Dal;

internal static class Config 
{
    internal const int StartAssignmentId = 1;
    private static int nextAssignmentId = StartAssignmentId;
    internal static int NextAssignmentId => nextAssignmentId++;

    internal const int StartCallId = 1;
    private static int nextCallId = StartCallId;
    internal static int NextCallId => nextCallId++;

    internal static DateTime Clock { get; set; } = DateTime.Now;
    internal static TimeSpan RiskRange { get; set; } = TimeSpan.FromHours(1);
    internal static void Reset()
    {
        nextAssignmentId = StartAssignmentId;
        nextCallId = StartCallId;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }
}
