namespace Dal;
using DalApi;

/// <summary>
/// Implementing functions of the Config entity
/// </summary>
internal class ConfigImplementation : IConfig
{
    public int NextCallId
    {
        get => Config.NextCallId;
    }
    public int NextAssignmentId
    {
        get => Config.NextAssignmentId;
    }
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
