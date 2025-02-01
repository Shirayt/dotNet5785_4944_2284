namespace BlImplementation;
using BlApi;

internal class CallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AddCall(BO.Call call)
    {
        throw new NotImplementedException();
    }

    public void CancelCallAssignment(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void DeleteCall(int callId)
    {
        throw new NotImplementedException();
    }

    public BO.Call GetCallDetails(int callId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<int> GetCallQuantitiesByStatus()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.CallInList> GetCallsList(Enum? filterField, object? filterValue, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, Enum? filterType, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsByVolunteer(int volunteerId, Enum? filterType, Enum? sortField)
    {
        throw new NotImplementedException();
    }

    public void MarkCallAsCompleted(int volunteerId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void SelectCallForTreatment(int volunteerId, int callId)
    {
        throw new NotImplementedException();
    }

    public void UpdateCallDetails(BO.Call call)
    {
        throw new NotImplementedException();
    }
}