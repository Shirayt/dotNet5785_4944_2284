namespace BO;

public class Call
{
    public int Id { get; init; }
    public CallType CallType { get; set; }//Emergency,Equipment,Doctor,Trainin
    public string? Description { get; set; }
    public string? FullAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime? MaxEndTime { get; set; }
    public CallStatus Status { get; set; }
    public List<BO.CallAssignInList>? CallAssignInList { get; set; }

    //public override string ToString() => this.ToStringProperty();
}
