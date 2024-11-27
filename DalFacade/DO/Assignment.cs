namespace DO;
public record Assignment
{
    public int Id { get; set; } = 0;
    public int CallId { get; set; }
    public int VolunteerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public AssignmentStatus? Status { get; set; }

    public Assignment(int callId, int volunteerId, DateTime startTime, DateTime? endTime = null, AssignmentStatus? status = null)
    {   CallId = callId;
        VolunteerId = volunteerId;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
    }
}

