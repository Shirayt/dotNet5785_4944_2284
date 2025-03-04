using BlApi;
using BO;
using Helpers;
namespace BlImplementation;
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void ForwardClock(TimeUnit unit)
    {
        DateTime currentClock = ClockManager.Now;
        DateTime newClock = unit switch
        {
            TimeUnit.Minute => currentClock.AddMinutes(1),
            TimeUnit.Hour => currentClock.AddHours(1),
            TimeUnit.Day => currentClock.AddDays(1),
            TimeUnit.Month => currentClock.AddMonths(1),
            TimeUnit.Year => currentClock.AddYears(1),
            _ => throw new NotImplementedException($"Invalid time unit {nameof(unit)}")
        };

        ClockManager.UpdateClock(newClock);
    }
    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public TimeSpan GetRiskRange()
    {
        return _dal.Config.RiskRange;
    }

    public void SetRiskRange(TimeSpan timeRange)
    {
        _dal.Config.RiskRange = timeRange;
    }
    public void InitializeDB()
    {
        _dal.ResetDB();
        DalTest.Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ResetDB()
    {
        _dal.ResetDB();
        _dal.Config.Reset();
        ClockManager.UpdateClock(ClockManager.Now);
    }
}



