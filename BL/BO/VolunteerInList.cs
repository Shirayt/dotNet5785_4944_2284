namespace BO;
public class VolunteerInList
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public bool IsActive { get; set; }
    public int AmountOfCompletedCalls { get; set; }
    public int AmountOfSelfCancelledCalls { get; set; }
    public int AmountOfExpiredCalls { get; set; }
    public int? CallInTreatmentId { get; set; }
    public override string ToString()
    {
        return
            "---- Volunteer In List Details ----\n" +
            $"Id: {Id}\n" +
            $"FullName: {FullName}\n" +
            $"IsActive: {IsActive}\n" +
            $"AmountOfCompletedCalls: {AmountOfCompletedCalls}\n" +
            $"AmountOfSelfCancelledCalls: {AmountOfSelfCancelledCalls}\n" +
            $"AmountOfExpiredCalls: {AmountOfExpiredCalls}\n" +
            $"CallInTreatmentId: {(CallInTreatmentId.HasValue ? CallInTreatmentId.Value.ToString() : "null")}";
    }
}
