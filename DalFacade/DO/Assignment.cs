namespace DO;

/// <summary>
/// Assignment entity definition
/// </summary>
public record Assignment
{
    public int Id { get; set; } = 0;
    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public AssignmentStatus? Status { get; set; } // Completed,SelfCancelled,ManagerCancelled,Expired

    /// Initializes a new Assignment with default values by calling the parameterized constructor.
    public Assignment() : this(
    0,                  // callId
    0,                  // volunteerId 
    DateTime.Now,       // startTime 
    null,               // endTime 
    null                // status
)
    { }


    /// Initializes a new Assignment with parameters.
    public Assignment(int callId, int volunteerId, DateTime startTime, DateTime? endTime = null, AssignmentStatus? status = null)
    {
        CallId = callId;
        VolunteerId = volunteerId;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
    }
}

