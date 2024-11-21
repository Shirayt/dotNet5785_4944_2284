namespace DO;

public enum AssignmentStatus
{
    Completed,
    Cancelled,
    InProgress
}

public record Assignment
{
    public int Id { get; set; }
    public int CalledId { get; set; }
    public int VolunteerId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AssignmentStatus Status { get; set; }
}

