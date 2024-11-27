namespace DO;

public enum CallType
{
    Emergency,
    Equipment,
    Doctor,
    Training
}
public record Call
{
    public int Id { get; set; } = 0;
    public CallType CallType { get; set; }
    public string? Description { get; set; }
    public string FullAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime? MaxEndTime { get; set; }

    public Call( CallType callType, string description, string fullAddress,
             double latitude, double longitude, DateTime openTime, DateTime? maxEndTime)
    {
        CallType = callType;
        Description = description;
        FullAddress = fullAddress;
        Latitude = latitude;
        Longitude = longitude;
        OpenTime = openTime;
        MaxEndTime = maxEndTime;
    }

}



