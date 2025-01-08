using DO;

namespace BO;
public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? FullName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public AssignmentStatus? Status { get; set; }
    //public override string ToString() => this.ToStringProperty();
}
