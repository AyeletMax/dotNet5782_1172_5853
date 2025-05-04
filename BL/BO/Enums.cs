namespace BO;
/// <summary>
/// Defines different roles within the system.
/// </summary>
public enum Role
{
    None,
    Manager,
    Volunteer
}

/// <summary>
/// Represents different distance calculation methods.
/// </summary>
public enum DistanceType
{
    None,
    Air,
    Walk,
    Drive
}

/// <summary>
/// Represents the types of calls handled in the system.
/// </summary>
public enum CallType
{
    None,
    MusicPerformance,
    MusicTherapy,
    SingingAndEmotionalSupport,
    GroupActivities,
    PersonalizedMusicCare
}

/// <summary>
/// Represents the different ways a call can be finished.
/// </summary>
public enum FinishCallType
{
    TakenCareOf,
    CanceledByVolunteer,
    CanceledByManager,
    Expired
}

/// <summary>
/// Represents the status of a call.
/// </summary>
public enum Status
{
    InProgress,       
    AtRisk,
    InProgressAtRisk,
    Opened,
    Closed,
    Expired
}

/// <summary>
/// Represents different time units used in the system.
/// </summary>
public enum TimeUnit
{
    MINUTE,
    HOUR,
    DAY,
    MONTH,
    YEAR
}

/// <summary>
/// Defines the sorting fields for volunteers.
/// </summary>
public enum VolunteerSortField
{
    None,
    Id,
    Name,
    TotalResponsesHandled,
    TotalResponsesCancelled,
    TotalExpiredResponses,
    SumOfCalls,
    SumOfCancellation,
    SumOfExpiredCalls
}

/// <summary>
/// Defines the fields available for listing call entries.
/// </summary>
public enum CallInListFields
{
    CallId,
    CallType,
    OpenTime,
    TimeRemainingToCall,
    LastVolunteer,
    CompletionTime,
    MyStatus,
    TotalAllocations,
}

/// <summary>
/// Defines the fields available for listing open calls.
/// </summary>
public enum OpenCallInListFields
{
    Id,
    CallType,
    Verbal_description,
    FullAddress,
    Start_time,
    Max_finish_time,
    CallDistance,
}


/// <summary>
/// Defines the fields available for listing closed calls.
/// </summary>
public enum ClosedCallInListFields
{
    Id,
    CallType,
    FullAddress,
    Opening_time,
    Start_time,
    End_time,
    EndType,
}