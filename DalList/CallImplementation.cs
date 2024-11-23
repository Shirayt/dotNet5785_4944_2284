namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        if (item.Id == 0)
        {
            Call newItem = item with { Id = Config.NextCallId };

            DataSource.Calls.Add(newItem);
        }
        else
        {
            if (DataSource.Calls.Any(call => call?.Id == item.Id))
            {
                throw new InvalidOperationException("אובייקט עם מזהה זה כבר קיים.");
            }

            DataSource.Calls.Add(item);
        }
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Call? Read(int id)
    {
        Call? existingItem = DataSource.Calls.FirstOrDefault(call => call?.Id == id);

        if (existingItem != null)
        {
            return existingItem;
        }

        return null;
    }
    public List<Call>? ReadAll()
    {
        if (DataSource.Calls.Count == 0)
        {
            return null;
        }

        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        throw new NotImplementedException();
    }
}
