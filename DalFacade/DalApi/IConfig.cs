namespace DalApi;

/// <summary>
///Access to the configuration functions definitions for managing system settings 
/// </summary>
public interface IConfig
{
    DateTime Clock { get; set; }
    void Reset();
    public TimeSpan RiskRange { get; set; }
}
