
namespace DO;

public enum AssignmentStatus
{
    Completed,        
    SelfCancelled,    
    ManagerCancelled, 
    Expired,
    //OpenAtRisk,
    //Open
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
