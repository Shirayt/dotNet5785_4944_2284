namespace DalApi;

/// <summary>
///Access to the configuration functions definitions for managing system settings 
/// </summary>
public interface IConfig
{
    public int NextCallId { get; }
    public int NextAssignmentId { get; }
    DateTime Clock { get; set; }
    void Reset();
    public TimeSpan RiskRange { get; set; }
}
