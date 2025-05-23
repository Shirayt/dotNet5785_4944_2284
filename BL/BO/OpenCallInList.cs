namespace BO;
public class OpenCallInList
{
    public int Id { get; init; }
    public CallType CallType { get; set; }
    public String? Description { get; set; }
    public string FullAddress { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime? MaxEndTime { get; set; }
    public double DistanceFromVolunteer { get; set; }

    public override string ToString()
    {
        return
            $"Id: {Id}\n" +
            $"CallType: {CallType}\n" +
            $"Description: {Description ?? "N/A"}\n" +
            $"FullAddress: {FullAddress}\n" +
            $"OpenTime: {OpenTime}\n" +
            $"MaxEndTime: {(MaxEndTime.HasValue ? MaxEndTime.Value.ToString() : "N/A")}\n" +
            $"DistanceFromVolunteer: {DistanceFromVolunteer} km";
    }

}
