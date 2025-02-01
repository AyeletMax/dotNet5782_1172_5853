namespace BO;
public enum Role
{
    Manager,
    Volunteer
}
public enum DistanceType
{
    Air,
    Walk,
    Drive
}
public enum CallType
{
    None,
    MusicPerformance,
    MusicTherapy,
    SingingAndEmotionalSupport,
    GroupActivities,
    PersonalizedMusicCare
}


public enum FinishCallType
{
    TakenCareOf,
    CanceledByVolunteer,
    CanceledByManager,
    Expired
}
public enum Status
{
    InProgress,       
    AtRisk,
    InProgressAtRisk,
    Opened,
    Closed,
    Expired
}

public enum TimeUnit
{
    MINUTE,
    HOUR,
    DAY,
    MONTH,
    YEAR
}
public enum VolunteerSortField
{
    Id,
    Name,
    TotalResponsesHandled,
    TotalResponsesCancelled,
    TotalExpiredResponses,
    SumOfCalls,
    SumOfCancellation,
    SumOfExpiredCalls
}

public enum CallInListFields
{
    CallId,
    CallType,
    Opening_time,
    TimeLeft,
    LastVolunteerName,
    TotalTime,
    CallStatus,
    TotalAssignments,
}

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