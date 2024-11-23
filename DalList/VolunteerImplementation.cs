
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
     
            if (DataSource.Volunteers.Any(volunteer => volunteer?.Id == item.Id))
            {
                throw new InvalidOperationException("אובייקט עם מזהה זה כבר קיים.");
            }

            DataSource.Volunteers.Add(item);
       
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        Volunteer? existingItem = DataSource.Volunteers.FirstOrDefault(volunteer => volunteer?.Id == id);

            if (existingItem != null)
            {
                return existingItem;
            }

            return null;
    }

    public List<Volunteer>? ReadAll()
    {
        if (DataSource.Volunteers.Count == 0)
        {
            return null;
        }

        return new List<Volunteer>(DataSource.Volunteers);
    }
    public void Update(Volunteer item)
    {
        throw new NotImplementedException();
    }
}
