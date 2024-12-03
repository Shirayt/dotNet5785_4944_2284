﻿namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class CallImplementation : ICall
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
                throw new Exception($"Call with ID={item.Id} already exist");
            }

            DataSource.Calls.Add(item);
        }
    }

    public void Delete(int id)
    {
        // מציאת האינדקס של האובייקט ברשימה עם המזהה המתאים
        int index = DataSource.Calls.FindIndex(call => call?.Id == id);

        if (index == -1)
            throw new Exception($"Call with ID {id} does not exist.");

        // הסרת האובייקט מהרשימה
        DataSource.Calls.RemoveAt(index);
    }


    public void DeleteAll()
    {
        DataSource.Calls.Clear();
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

    public Call? Read(Func<Call, bool> filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

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
        // מציאת האינדקס של האובייקט הקיים עם אותו ID
        int index = DataSource.Calls.FindIndex(call => call?.Id == item.Id);

        if (index == -1)
            throw new Exception($"Call with ID {item.Id} does not exist.");

        // החלפת האובייקט הקיים באובייקט החדש
        DataSource.Calls[index] = item;
    }

}
