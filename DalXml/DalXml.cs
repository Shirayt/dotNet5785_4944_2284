using DalApi;
namespace Dal;
/// <summary>
/// Manages access to system XML data
/// </summary>
//stage 3
sealed public class DalXml : IDal
{
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
