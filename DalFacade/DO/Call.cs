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
    int Id { get; set; }
    CallType CallType { get; set; }
    string? Description { get; set; }
    string FullAddress { get; set; }
    double Latitude { get; set; }
    double Longitude { get; set; }
    DateTime OpenTime { get; set; }
    DateTime? MaxEndTime { get; set; }

    public Call() { }
}



