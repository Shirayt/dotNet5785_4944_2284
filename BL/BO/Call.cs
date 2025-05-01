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

    //public override string ToString()
    //{
    //    return
    //        "---- Call Details ----\n" +
    //        $"Id: {Id}\n" +
    //        $"CallType: {CallType}\n" +
    //        $"Description: {(Description ?? "Not Provided")}\n" +
    //        $"FullAddress: {(FullAddress ?? "Not Provided")}\n" +
    //        $"Latitude: {(Latitude.HasValue ? Latitude.Value.ToString() : "Not Provided")}\n" +
    //        $"Longitude: {(Longitude.HasValue ? Longitude.Value.ToString() : "Not Provided")}\n" +
    //        $"OpenTime: {OpenTime}\n" +
    //        $"MaxEndTime: {(MaxEndTime.HasValue ? MaxEndTime.Value.ToString() : "Not Provided")}\n" +
    //        $"Status: {Status}\n" +
    //        $"CallAssignInList: {string.Join(", ", CallAssignInList ?? new List<CallAssignInList>())}";
    //}

    /// <summary>
    /// Provides a string representation of the Call object based on its properties.
    /// </summary>
    /// <returns>A string representing the Call object.</returns>
    public override string ToString() => this.ToStringProperty();

}
