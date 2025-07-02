namespace Dal;
using DalApi;
using DO;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementing the CRUD functions on the Assignment entity from XML data list
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment newItem = item with { Id = Config.NextAssignmentId };
        Assignments.Add(newItem);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        Assignment? existingItem = Assignments.FirstOrDefault(assignment => assignment?.Id == id);

        if (existingItem != null)
        {
            return existingItem;
        }

        throw new DalDoesNotExistException($"Assignment with ID {id} does not exist.");
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment? existingItem = Assignments.FirstOrDefault(assignment => filter(assignment));

        if (existingItem != null)
        {
            return existingItem;
        }

        return null;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.Count == 0)
        {
            return null;
        }
        return filter == null
            ? Assignments.Select(item => item)
            : Assignments.Where(filter);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist");
        Assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);
    }
}