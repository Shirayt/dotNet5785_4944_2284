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

    public override string ToString() => this.ToStringProperty();

}
