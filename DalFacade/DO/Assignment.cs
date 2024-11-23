namespace DO;

public enum AssignmentStatus
{
    Completed,
    Cancelled,
    InProgress
}

public record Assignment
{   
    int Id { get; set; }
    int CalledId { get; set; }
    int VolunteerId { get; set; }
    DateTime StartTime { get; set; }
    DateTime? EndTime { get; set; }
    AssignmentStatus? Status { get; set; }

    public Assignment() { }
}

