namespace BO;

public class CallInList
{
    public int Id { get; init; }
    public int CallId { get; init; }
    public CallType CallType { get; set; }
    public DateTime OpenTime { get; set; }
    public TimeSpan? RestTimeForCall { get; set; }
    public string? LastVolunteerName { get; set; }
    public TimeSpan? TreatmentCompletionTime { get; set; }
    public CallStatus Status { get; set; }
    public int AllocationsAmount { get; set; }

    public override string ToString()
    {
        return
            $"Id: {Id}\n" +
            $"CallId: {CallId}\n" +
            $"CallType: {CallType}\n" +
            $"OpenTime: {OpenTime}\n" +
            $"RestTimeForCall: {(RestTimeForCall.HasValue ? RestTimeForCall.Value.ToString() : "N/A")}\n" +
            $"LastVolunteerName: {LastVolunteerName ?? "N/A"}\n" +
            $"RestTimeForTreatment: {(TreatmentCompletionTime.HasValue ? TreatmentCompletionTime.Value.ToString() : "N/A")}\n" +
            $"Status: {Status}\n" +
            $"AllocationsAmount: {AllocationsAmount}";
    }
}
