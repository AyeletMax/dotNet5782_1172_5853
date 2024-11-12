

namespace DO;
/// <summary>
/// Call Entity
/// </summary>
/// <param name="ID">unique Id that is automaticaly updated</param>
/// <param name="VerbalDescription">verble description of the call</param>
/// <param name="Address">full address of the call</param>
/// <param name="Latitude">A number indicating how far a point on Earth is south or north of the equator.</param>
/// <param name="Longitude">A number indicating how far a point on Earth is east or west of the equator.</param>
/// <param name="OpenTime">time(date and hour) that the call has been opened</param>
/// <param name="MaxFinishTime">time (date and hour) that the call has to finish</param>
public record Call
(
    CallType MyCallType,
    string VerbalDescription,
    string Address,
    double Latitude,
    double Longitude,
    DateTime OpenTime,
    DateTime? MaxFinishTime = null
)
{
    internal const int StartId = 1;
    private static int id = StartId;
    internal static int Id { get => id++; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public Call() : this(default(CallType), "", "", 0.0, 0.0, DateTime.MinValue, null) { }
}