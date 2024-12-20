namespace DO;

/// <summary>
///  Volunteer entity definition
/// </summary>
public record Volunteer
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? CurrentFullAddress { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public Role Role { get; set; } //Manager,Volunteer 
    public bool IsActive { get; set; }
    public double? MaxDistanceForCall { get; set; }
    public DistanceType DistanceType { get; set; } = DistanceType.Air; //  Air,Walk,Drive

    /// Initializes a new Volunteer with default values by calling the parameterized constructor.
    public Volunteer() : this(
    0,                // id
    "",               // fullName
    "",               // phoneNumber
    "",               // email
    null,             // currentFullAddress
    null,             // latitude
    null,             // longitude
    Role.Volunteer,   // role
    false,            // isActive
    null,             // maxDistanceForCall
    DistanceType.Air, // distanceType
    null)             // password
    { }

    /// Initializes a new Volunteer with parameters.
    public Volunteer(int id, string fullName, string phoneNumber, string email, string? currentFullAddress, double? latitude, double? longitude, Role role, bool isActive, double? maxDistanceForCall, DistanceType distanceType = DistanceType.Air, string? password = null)
    {
        Id = id;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Email = email;
        Password = password;
        CurrentFullAddress = currentFullAddress;
        Latitude = latitude;
        Longitude = longitude;
        Role = role;
        IsActive = isActive;
        MaxDistanceForCall = maxDistanceForCall;
        DistanceType = distanceType;
    }

}
