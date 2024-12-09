namespace Dal;
using DalApi;
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
