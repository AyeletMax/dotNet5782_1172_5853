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

