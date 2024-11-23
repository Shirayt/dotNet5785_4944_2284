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
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }


}

