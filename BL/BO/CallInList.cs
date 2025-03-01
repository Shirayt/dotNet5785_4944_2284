namespace BO;

public class CallInList
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallType CallType { get; set; }
    public DateTime OpenTime { get; set; }
    public TimeSpan? RestTimeForCall { get; set; }
    public string? LastVolunteerName { get; set; }
    public TimeSpan? RestTimeForTreatment { get; set; }
    public CallStatus Status { get; set; }
    public int AllocationsAmount { get; set; }
    //public override string ToString() => this.ToStringProperty();
}
