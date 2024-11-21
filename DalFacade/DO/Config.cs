namespace DO;

public record Config
{
    public int NextCallId { get; set; }
    public int NextAssignmentId { get; set; }
    public DateTime Clock { get; set; }
    public TimeSpan RiskRange { get; set; }
}