
namespace BlApi;
/// <summary>
/// Public interface methods to be invoked via the view or via BlTest
/// </summary>
public interface IVolunteer
{
    /// <summary>
        /// Logs into the system and returns the user's role.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns>The role of the user.</returns>
        /// <exception cref="Exception">Thrown if the user does not exist or the password is incorrect.</exception>
        string LoginVolunteerToSystem(string username, string password);

    /// <summary>
    /// Requests a filtered and sorted list of volunteers.
    /// </summary>
    /// <param name="isActive">Filter by active/inactive volunteers. Nullable.</param>
    /// <param name="sortBy">Sort by a specific field in VolunteerInList. Nullable.</param>
    /// <returns>A sorted and filtered collection of volunteers in list format.</returns>
    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null,Enum ? sortBy = null);

    /// <summary>
    /// Requests the details of a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer (e.g., their national ID).</param>
    /// <returns>A BO.Volunteer object containing the details of the volunteer.</returns>
    /// <exception cref="Exception">Thrown if the volunteer does not exist.</exception>
    BO.Volunteer GetVolunteerDetails(int volunteerId);

    /// <summary>
    /// Updates the details of a volunteer.
    /// </summary>
    /// <param name="requesterId">The ID of the requester (manager or the volunteer themselves).</param>
    /// <param name="volunteer">A BO.Volunteer object with updated values.</param>
    /// <exception cref="Exception">Thrown if validation fails or the requester is unauthorized.</exception>
    void UpdateVolunteerDetails(int volunteerId, BO.Volunteer volunteer);

    /// <summary>
    /// Requests the deletion of a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer to be deleted.</param>
    /// <exception cref="Exception">Thrown if the volunteer cannot be deleted or does not exist.</exception>
    void DeleteVolunteer(int volunteerId);

    /// <summary>
    /// Adds a new volunteer.
    /// </summary>
    /// <param name="volunteer">A BO.Volunteer object with complete details of the new volunteer.</param>
    /// <exception cref="Exception">Thrown if validation fails or a volunteer with the same ID already exists.</exception>
    void AddVolunteer(BO.Volunteer volunteer);
}

