namespace Dal;
using DalApi;
using DO;
using System.Data;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    static Volunteer getVolunteer(XElement v)//convert xelement to volunteer type
    {
        return new DO.Volunteer()
        {
            Id = v.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            FullName = (string?)v.Element("FullName") ?? "",
            PhoneNumber = (string?)v.Element("PhoneNumber") ?? null,
            Email = (string?)v.Element("Email") ?? null,
            Password = (string?)v.Element("Password") ?? null,
            CurrentFullAddress = (string?)v.Element("CurrentFullAddress") ?? null,
            Latitude = (double?)v.Element("Latitude") ?? null,
            Longitude = (double?)v.Element("Longitude") ?? null,
            Role = v.ToEnumNullable<Role>("Role") ?? Role.Volunteer,
            IsActive = (bool?)v.Element("IsActive") ?? false,
            MaxDistanceForCall = (double?)v.Element("MaxDistanceForCall") ?? null,
            DistanceType = v.ToEnumNullable<DistanceType>("DistanceType") ?? DistanceType.Air
        };

    }
    private XElement createVolunteerElement(Volunteer item)//convert volunteer type to xelement
    {
        return new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("FullName", item.FullName),
            new XElement("PhoneNumber", item.PhoneNumber ?? string.Empty),
            new XElement("Email", item.Email ?? string.Empty),
            new XElement("Password", item.Password ?? string.Empty),
            new XElement("CurrentFullAddress", item.CurrentFullAddress ?? string.Empty),
            new XElement("Latitude", item.Latitude?.ToString() ?? string.Empty),
            new XElement("Longitude", item.Longitude?.ToString() ?? string.Empty),
            new XElement("Role", item.Role.ToString()),
            new XElement("IsActive", item.IsActive.ToString()),
            new XElement("MaxDistanceForCall", item.MaxDistanceForCall?.ToString() ?? string.Empty),
            new XElement("DistanceType", item.DistanceType.ToString())
        );
    }
    public void Create(Volunteer item)
    {
        XElement volunteersRootElement = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        if (volunteersRootElement.Elements("Volunteer")
                                 .Any(v => (int?)v.Element("Id") == item.Id))
        {
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        }

        volunteersRootElement.Add(createVolunteerElement(item));

        XMLTools.SaveListToXMLElement(volunteersRootElement, Config.s_volunteers_xml);
    }
    public Volunteer? Read(int id)
    {
        XElement? VolunteerElement =
        XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(v => (int?)v.Element("Id") == id);
        return VolunteerElement is null ? null : getVolunteer(VolunteerElement);
    }
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v)).FirstOrDefault(filter);
    }
    public IEnumerable<Volunteer>? ReadAll(Func<Volunteer, bool>? filter = null)
    {
        XElement volunteersRootElement = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        var volunteers = volunteersRootElement
            .Elements("Volunteer")
            .Select(v => getVolunteer(v)); // using getVolunteer function

        return filter == null
            ? volunteers
            : volunteers.Where(filter);
    }
    public void Update(Volunteer item)
    {
        XElement volunteersRootElement = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElement.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.Id)
        ?? throw new DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist"))
                .Remove();

        volunteersRootElement.Add(new XElement("Volunteer", createVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteersRootElement, Config.s_volunteers_xml);
    }
    public void Delete(int id)
    {
        XElement volunteersRootElement = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        XElement volunteerElement = volunteersRootElement.Elements("Volunteer")
            .FirstOrDefault(v => (int?)v.Element("Id") == id);

        if (volunteerElement == null)
            throw new DalDoesNotExistException($"Volunteer with ID {id} does not exist.");

        volunteerElement.Remove();

        XMLTools.SaveListToXMLElement(volunteersRootElement, Config.s_volunteers_xml);
    }
    public void DeleteAll()
    {
        XElement volunteersRootElement = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        volunteersRootElement.RemoveAll();
        XMLTools.SaveListToXMLElement(volunteersRootElement, Config.s_volunteers_xml);
    }

}
