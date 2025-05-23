
namespace BO;
public class CallAssignInList
{
    public int? VolunteerId { get; init; }
    public string? FullName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public AssignmentStatus? Status { get; set; }
    public override string ToString()
    {
        return
            $"VolunteerId: {(VolunteerId.HasValue ? VolunteerId.ToString() : "N/A")}\n" +
            $"FullName: {FullName ?? "N/A"}\n" +
            $"StartTime: {StartTime}\n" +
            $"EndTime: {(EndTime.HasValue ? EndTime.Value.ToString() : "N/A")}\n" +
            $"Status: {(Status.HasValue ? Status.ToString() : "N/A")}";
    }
}
