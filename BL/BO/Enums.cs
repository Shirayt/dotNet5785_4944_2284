
namespace BO;
public enum AssignmentStatus
{
    Completed,
    SelfCancelled,
    ManagerCancelled,
    Expired
}
public enum CallType
{
    Emergency,
    Equipment,
    Doctor,
    Training
}

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

public enum CallStatus
{
    Open,
    InProcessing,
    Closed,
    Expired,
    OpenAtRisk,
}

public enum StatusCallInProgress
{
    InProcessing,
    InProcessingInRisk
}

public enum VolunteerSortOption
{
    ByName,
    ByCompletedCalls,
    MaxDistanceForCall
}
public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}




