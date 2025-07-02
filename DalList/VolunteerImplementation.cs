namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Implementing the CRUD functions on the Volunteer entity
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Volunteer item)
    {

        if (DataSource.Volunteers.Any(volunteer => volunteer?.Id == item.Id))
        {
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exist");
        }

        DataSource.Volunteers.Add(item);

    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(int id)
    {
        Volunteer? existingItem = DataSource.Volunteers.FirstOrDefault(volunteer => volunteer?.Id == id);

        if (existingItem != null)
        {
            return existingItem;
        }

        throw new DalDoesNotExistException($"Volunteer with ID {id} does not exist.");
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        Volunteer? existingItem = DataSource.Volunteers.FirstOrDefault(volunteer => filter(volunteer));

        if (existingItem != null)
        {
            return existingItem;
        }

        return null;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Volunteer>? ReadAll(Func<Volunteer, bool>? filter = null)
    {
        if (DataSource.Volunteers.Count == 0)
        {
            return null;
        }
        return filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Volunteer item)
    {
        // Find the index of the object in the list with the matching ID
        int index = DataSource.Volunteers.FindIndex(volunteer => volunteer?.Id == item.Id);

        if (index == -1)
            throw new DalDoesNotExistException($"Volunteer with ID {item.Id} does not exist.");

        // Replace the existing object with the new object
        DataSource.Volunteers[index] = item;
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        // Find the index of the object in the list with the matching ID
        int index = DataSource.Volunteers.FindIndex(volunteer => volunteer?.Id == id);

        if (index == -1)
            throw new DalDoesNotExistException($"Volunteer with ID {id} does not exist.");

        // Remove the object from the list
        DataSource.Volunteers.RemoveAt(index);
    }
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }
}