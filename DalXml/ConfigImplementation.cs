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
    public TimeSpan RiskRange
    {
        get => Config.RiskRange;
        set => Config.RiskRange = value;
    }
    public void Reset()
    {
        Config.Reset();
    }

}
