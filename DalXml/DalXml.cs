using DalApi;
namespace Dal;
/// <summary>
/// Manages access to system XML data
/// </summary>
//stage 3
sealed internal class DalXml : IDal
{
    public static IDal Instance { get; } = new DalXml();
    private DalXml() { }
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public ICall Call { get; } = new CallImplementation();
    public IVolunteer Volunteer { get; } = new VolunteerImplementation();
    public IConfig Config { get; } = new ConfigImplementation();
    public void ResetDB()
    {
        Assignment.DeleteAll();

        Call.DeleteAll();

        Volunteer.DeleteAll();

        Config.Reset();
    }
}
