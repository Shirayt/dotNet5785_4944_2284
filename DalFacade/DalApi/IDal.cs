namespace DalApi;
/// <summary>
/// An interface to union all kinds of entities to be accessible by one entity
/// </summary>
public interface IDal
{
    IAssignment Assignment { get; }
    ICall Call { get; }
    IVolunteer Volunteer { get; }
    IConfig Config { get; }
    void ResetDB();
}
