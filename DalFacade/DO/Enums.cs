namespace DO;
/// <summary>
/// Represents the different roles in the system.
/// </summary>
public enum Role
{
    Manager,
    Volunteer
}

/// <summary>
/// Represents different types of distances used in the system.
/// </summary>
public enum DistanceType
{
    Air,
    Walk,
    Drive
}

/// <summary>
/// Represents the different types of calls in the system.
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
/// Represents the possible finish statuses for a call.
/// </summary>
public enum FinishCallType
{
    TakenCareOf,
    CanceledByVolunteer,
    CanceledByManager,
    Expired
}
