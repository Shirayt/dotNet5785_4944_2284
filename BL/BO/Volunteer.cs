namespace BO;
public class Volunteer
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? CurrentFullAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Role Role { get; set; } 
    public bool IsActive { get; set; }
    public double? MaxDistanceForCall { get; set; }
    public DistanceType DistanceType { get; set; } 
    public int AmountOfCompletedCalls { get; set; }
    public int AmountOfSelfCancelledCalls { get; set; }
    public int AmountOfExpiredCalls { get; set; }
    public BO.CallInProgress? callInProgress { get; set; }
    //public override string ToString() => this.ToStringProperty();

}