namespace Dal;
using DalApi;

/// <summary>
/// Implementing functions of the Config entity for XML data 
/// </summary>
internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }
    public TimeSpan RiskRange { get; set; }
    public void Reset()
    {
        Config.Reset();
    }

}
