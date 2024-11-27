
namespace DalApi;
using DO;
public interface IConfig
{
    DateTime Clock { get; set; }
    void Reset();
    public TimeSpan RiskRange { get; set; }
}
