using BlApi;
using BO;
using DalApi;
using Helpers;
using System.Threading;
namespace BlImplementation;
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }
    public void ForwardClock(TimeUnit unit)
    {
        DateTime currentClock = AdminManager.Now;
        DateTime newClock = unit switch
        {
            TimeUnit.Minute => currentClock.AddMinutes(1),
            TimeUnit.Hour => currentClock.AddHours(1),
            TimeUnit.Day => currentClock.AddDays(1),
            TimeUnit.Month => currentClock.AddMonths(1),
            TimeUnit.Year => currentClock.AddYears(1),
            _ => throw new BlNotImplementedException($"Invalid time unit {nameof(unit)}")
        };

        AdminManager.UpdateClock(newClock);
    }
    public TimeSpan GetRiskRange()
    {
        return AdminManager.RiskRange;
    }
    public void SetRiskRange(TimeSpan timeRange)
    {
        AdminManager.RiskRange = timeRange;
    }
    //public void ResetDB()
    //{
    //    _dal.ResetDB();
    //    _dal.Config.Reset();
    //    AdminManager.UpdateClock(AdminManager.Now);
    //}
    //public void InitializeDB()
    //{
    //    _dal.ResetDB();
    //    DalTest.Initialization.Do();
    //    AdminManager.UpdateClock(AdminManager.Now);
    //}


    public void InitializeDB()
    {
        AdminManager.InitializeDB();
    }
    public void ResetDB()
    {
        AdminManager.ResetDB();
    }


    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5

}



