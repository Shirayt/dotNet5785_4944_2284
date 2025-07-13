namespace BlApi;

/// <summary>
/// Public interface methods to be invoked via the view or via BlTest
/// </summary>
public interface ICall : IObservable
{
    /// <summary>
    /// Requests the quantities of calls grouped by their status.
    /// </summary>
    /// <returns>A collection of integers representing the count of calls per status.</returns>
    IEnumerable<int> GetCallQuantitiesByStatus();

    /// <summary>
    /// Requests a filtered and sorted list of calls.
    /// </summary>
    /// <param name="filterField">Filter by a specific field. Nullable.</param>
    /// <param name="filterValue">Filter value. Nullable.</param>
    /// <param name="sortField">Sort by a specific field. Nullable.</param>
    /// <returns>A sorted and filtered collection of calls in list format.</returns>
    IEnumerable<BO.CallInList> GetCallsList(BO.CallInListFields? filterField, object? filterValue, BO.CallInListFields? sortField);

    /// <summary>
    /// Requests the details of a specific call.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>A BO.Call object containing the details of the call.</returns>
    /// <exception cref="Exception">Thrown if the call does not exist.</exception>
    BO.Call GetCallDetails(int callId);

    /// <summary>
    /// Updates the details of a call.
    /// </summary>
    /// <param name="call">A BO.Call object with updated values.</param>
    /// <exception cref="Exception">Thrown if validation fails.</exception>
    Task UpdateCallDetails(BO.Call call);

    /// <summary>
    /// Requests the deletion of a call.
    /// </summary>
    /// <param name="callId">The ID of the call to be deleted.</param>
    /// <exception cref="Exception">Thrown if the call cannot be deleted or does not exist.</exception>
    void DeleteCall(int callId);

    /// <summary>
    /// Adds a new call.
    /// </summary>
    /// <param name="call">A BO.Call object with complete details of the new call.</param>
    /// <exception cref="Exception">Thrown if validation fails or a call with the same ID already exists.</exception>
    Task AddCall(BO.Call call);

    /// <summary>
    /// Requests a list of closed calls handled by a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="filterType">Filter by a specific field. Nullable.</param>
    /// <param name="sortField">Sort by a specific field. Nullable.</param>
    /// <returns>A collection of closed calls handled by the volunteer.</returns>
    IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.FilterAndSortByFields? filterType, object? filterValue, BO.FilterAndSortByFields? sortField);

    /// <summary>
    /// Requests a list of open calls available for a specific volunteer to handle.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <param name="filterType">Filter by a specific field. Nullable.</param>
    /// <param name="sortField">Sort by a specific field. Nullable.</param>
    /// <returns>A collection of open calls available for the volunteer.</returns>
    IEnumerable<BO.OpenCallInList> GetOpenCallsByVolunteer(int volunteerId, BO.FilterAndSortByFields? filterType, BO.FilterAndSortByFields? sortField);

    /// <summary>
    /// Marks a call as completed by a volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer completing the call.</param>
    /// <param name="assignmentId">The ID of the call assignment.</param>
    /// <exception cref="Exception">Thrown if the operation fails or the IDs are invalid.</exception>
    void MarkCallAsCompleted(int volunteerId, int assignmentId);

    /// <summary>
    /// Cancels a call assignment.
    /// </summary>
    /// <param name="requesterId">The ID of the requester canceling the assignment.</param>
    /// <param name="assignmentId">The ID of the call assignment.</param>
    /// <exception cref="Exception">Thrown if the operation fails or the IDs are invalid.</exception>
    void CancelCallAssignment(int volunteerId, int assignmentId);

    /// <summary>
    /// Handles the selection of a call for treatment.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer handling the call.</param>
    /// <param name="callId">The ID of the call to be handled.</param>
    /// <exception cref="Exception">
    /// Thrown if the call has already been handled, is assigned to another volunteer,
    /// has expired, or if any other validation fails.
    /// </exception>
    void SelectCallForTreatment(int volunteerId, int callId);

    /// <summary>
    /// Returns a list of all assignments for a given call ID.
    /// </summary>
    /// <param name="callId">The ID of the call.</param>
    /// <returns>A collection of assignments related to the call.</returns>
    IEnumerable<BO.CallAssignInList> GetAssignmentsByCallId(int callId);

}

