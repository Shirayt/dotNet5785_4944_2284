
namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {

        if (DataSource.Volunteers.Any(volunteer => volunteer?.Id == item.Id))
        {
            throw new Exception($"Volunteer with ID={item.Id} already exist");
        }

        DataSource.Volunteers.Add(item);

    }

    public void Delete(int id)
    {
        // מציאת האינדקס של האובייקט ברשימה עם המזהה המתאים
        int index = DataSource.Volunteers.FindIndex(volunteer => volunteer?.Id == id);

        if (index == -1)
            throw new Exception($"Volunteer with ID {id} does not exist.");

        // הסרת האובייקט מהרשימה
        DataSource.Volunteers.RemoveAt(index);
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
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

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        if (filter == null)
            throw new ArgumentNullException(nameof(filter));

        Volunteer? existingItem = DataSource.Volunteers.FirstOrDefault(volunteer => filter(volunteer));

        if (existingItem != null)
        {
            return existingItem;
        }

        return null;
    }

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

    public void Update(Volunteer item)
    {
       // מציאת האינדקס של האובייקט הקיים עם אותו ID
        int index = DataSource.Volunteers.FindIndex(volunteer => volunteer?.Id == item.Id);

        if (index == -1)
            throw new Exception($"Volunteer with ID {item.Id} does not exist.");

        // החלפת האובייקט הקיים באובייקט החדש
        DataSource.Volunteers[index] = item;
    }

}
