namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        if (item.Id == 0)
        {
            Assignment newItem = item with { Id = Config.NextAssignmentId };

            DataSource.Assignments.Add(newItem);
        }
        else
        {
            if (DataSource.Assignments.Any(assignment => assignment?.Id == item.Id))
            {
                throw new InvalidOperationException("אובייקט עם מזהה זה כבר קיים.");
            }

            DataSource.Assignments.Add(item);
        }
    }


    public void Delete(int id)
    {
        // מציאת האינדקס של האובייקט ברשימה עם המזהה המתאים
        int index = DataSource.Assignments.FindIndex(assignment => assignment?.Id == id);

        if (index == -1)
            throw new KeyNotFoundException($"Assignment with ID {id} does not exist.");

        // הסרת האובייקט מהרשימה
        DataSource.Assignments.RemoveAt(index);
    }


    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        Assignment? existingItem = DataSource.Assignments.FirstOrDefault(assignment => assignment?.Id == id);

        if (existingItem != null)
        {
            return existingItem;
        }

        return null;
    }

    public List<Assignment>? ReadAll()
    {
        if (DataSource.Assignments.Count == 0)
        {
            return null;
        }

        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        // מציאת האינדקס של האובייקט הקיים עם אותו ID
        int index = DataSource.Assignments.FindIndex(assignment => assignment?.Id == item.Id);

        if (index == -1)
            throw new KeyNotFoundException($"Assignment with ID {item.Id} does not exist.");

        // החלפת האובייקט הקיים באובייקט החדש
        DataSource.Assignments[index] = item;
    }



}

