
namespace BlApi;
/// <summary>
/// Public interface methods to be invoked via the view or via BlTest
/// </summary>
public interface IAdmin
{
    void InitializeDB();
    void ResetDB();
    int GetRiskRange();
    DateTime GetClock();
    void ForwardClock(BO.TimeUnit unit);
}
