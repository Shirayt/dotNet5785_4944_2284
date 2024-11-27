namespace Dal;
using DalApi;
using DO;
internal class ConfigImplementation : IConfig
{
    public DateTime Clock
    {

        get => Config.Clock;
        set => Config.Clock = value;

    }
    public TimeSpan RiskRang { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void Reset()
    {
        Config.Reset();
    }

}
