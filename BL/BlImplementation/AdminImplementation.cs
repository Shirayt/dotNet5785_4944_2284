using BlApi;
using BO;
using DalApi;
using Helpers;
using System.Threading;
namespace BlImplementation;
internal class AdminImplementation : IAdmin
{
    public DateTime GetClock()
    {
        return AdminManager.Now;
    }
    public void ForwardClock(TimeUnit unit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

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
        CallManager.Observers.NotifyListUpdated();
        VolunteerManager.Observers.NotifyListUpdated();
    }
    public void SetRiskRange(TimeSpan timeRange)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.RiskRange = timeRange;
        CallManager.Observers.NotifyListUpdated();
    }
    public TimeSpan GetRiskRange()
    {
        return AdminManager.RiskRange;
    }

    public void InitializeDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.InitializeDB();
    }
    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
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


    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }

    public void StopSimulator()
    => AdminManager.Stop(); //stage 7

}


