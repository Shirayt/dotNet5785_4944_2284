namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

/// <summary>
/// Implementing the CRUD functions on the Call entity
/// </summary>
internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        Call newItem = item with { Id = Config.NextCallId };

        DataSource.Calls.Add(newItem);
    }
    public Call? Read(int id)
    {
        Call? existingItem = DataSource.Calls.FirstOrDefault(call => call?.Id == id);

        if (existingItem != null)
        {
            return existingItem;
        }

        throw new DalDoesNotExistException($"Call with ID {id} does not exist.");
    }
    public Call? Read(Func<Call, bool> filter)
    {
        Call? existingItem = DataSource.Calls.FirstOrDefault(call => filter(call));

        if (existingItem != null)
        {
            return existingItem;
        }

        return null;
    }
    public IEnumerable<Call>? ReadAll(Func<Call, bool>? filter = null)
    {
        if (DataSource.Calls.Count == 0)
        {
            return null;
        }
        return filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);
    }
    public void Update(Call item)
    {
        // Find the index of the object in the list with the matching ID
        int index = DataSource.Calls.FindIndex(call => call?.Id == item.Id);

        if (index == -1)
            throw new DalDoesNotExistException($"Call with ID {item.Id} does not exist.");

        // Replace the existing object with the new object
        DataSource.Calls[index] = item;
    }
    public void Delete(int id)
    {
        // Find the index of the object in the list with the matching ID
        int index = DataSource.Calls.FindIndex(call => call?.Id == id);

        if (index == -1)
            throw new DalDoesNotExistException($"Call with ID {id} does not exist.");

        // Remove the object from the list
        DataSource.Calls.RemoveAt(index);
    }
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }
}
