
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementing the CRUD functions on the Assignment entity
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Assignment item)
    {
        Assignment newItem = item with { Id = Config.NextAssignmentId };
        DataSource.Assignments.Add(newItem);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(int id)
    {
        Assignment? existingItem = DataSource.Assignments.FirstOrDefault(assignment => assignment?.Id == id);

        if (existingItem != null)
        {
            return existingItem;
        }

        throw new DalDoesNotExistException($"Assignment with ID {id} does not exist.");
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        Assignment? existingItem = DataSource.Assignments.FirstOrDefault(assignment => filter(assignment));

        if (existingItem != null)
        {
            return existingItem;
        }

        return null;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Assignment>? ReadAll(Func<Assignment, bool>? filter = null)
    {
        if (DataSource.Assignments.Count == 0)
        {
            return null;
        }
        return filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Assignment item)
    {
        // Find the index of the object in the list with the matching ID
        int index = DataSource.Assignments.FindIndex(assignment => assignment?.Id == item.Id);

        if (index == -1)
            throw new DalDoesNotExistException($"Assignment with ID {item.Id} does not exist.");

        // Replace the existing object with the new object
        DataSource.Assignments[index] = item;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        // Find the index of the object in the list with the matching ID
        int index = DataSource.Assignments.FindIndex(assignment => assignment?.Id == id);

        if (index == -1)
            throw new DalDoesNotExistException($"Assignment with ID {id} does not exist.");

        // Remove the object from the list
        DataSource.Assignments.RemoveAt(index);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }
}