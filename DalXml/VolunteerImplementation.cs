namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

internal class VolunteerImplementation : IVolunteer
{
    static Volunteer getVolunteer(XElement v)
    {
        return new DO.Volunteer()
        {
            Id = v.ToIntNullable("Id") ?? throw new FormatException("can't convert id"),
            FirstName = (string?)v.Element("FirstName") ?? "",
            LastName = (string?)v.Element("LastName") ?? "",
            Phone = (string?)v.Element("Phone") ?? "",
            Email = (string?)v.Element("Email") ?? "",
            Active = (bool?)v.Element("IsActive") ?? false,
            MyRole = Enum.TryParse<Role>((string)v.Element("Role") ?? "", out var role) ? role : Role.Volunteer,
            Password = (string?)v.Element("Password") ?? "",
            Address = (string?)v.Element("Address") ?? "",
            Latitude = v.ToDoubleNullable("Latitude") ?? throw new FormatException("can't convert Latitude"),
            Longitude = v.ToDoubleNullable("Longitude") ?? throw new FormatException("can't convert Longitude"),
            LargestDistance = v.ToDoubleNullable("LargestDistance") ?? throw new FormatException("can't convert LargestDistance"),
            MyDistanceType= Enum.TryParse<DistanceType>((string)v.Element("DistanceType") ?? "", out var distance) ? distance : DistanceType.Air,
        };
    }
    public void Create(Volunteer item)
    {
        XElement volunteerElement = CreateVolunteerElement(item);
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        volunteersRootElem.Add(volunteerElement);
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }
    private XElement CreateVolunteerElement(Volunteer item)
    {
        return new XElement("Volunteer",
            new XElement("Id", item.Id),
            new XElement("FirstName", item.FirstName),
            new XElement("LastName", item.LastName),
            new XElement("Phone", item.Phone),
            new XElement("Email", item.Email),
            new XElement("IsActive", item.Active),
            new XElement("Role", item.MyRole.ToString()),
            new XElement("Password", item.Password),
            new XElement("Address", item.Address),
            new XElement("Latitude", item.Latitude),
            new XElement("Longitude", item.Longitude),
            new XElement("LargestDistance", item.LargestDistance),
            new XElement("DistanceType", item.MyDistanceType.ToString()) 
        );
    }

    public void Delete(int id)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        XElement volunteerElem = volunteersRootElem.Elements("Volunteer")
            .FirstOrDefault(v => (int)v.Element("Id")! == id)!;
        if ((volunteerElem) == null)
        {
            throw new DalDoesNotExistException($"Volunteer with ID={id} not found.");
        }
        volunteerElem.Remove();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    public void DeleteAll()
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        volunteersRootElem.RemoveAll();
        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }

    public Volunteer? Read(int id)
    {
        XElement? volunteerElem =
        XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().FirstOrDefault(st => (int?)st.Element("Id") == id);
        return volunteerElem is null ? null : getVolunteer(volunteerElem);
    }

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml).Elements().Select(v => getVolunteer(v)).FirstOrDefault(filter);
    }

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);
        var volunteers = volunteersRootElem.Elements("Volunteer")
            .Select(v => new Volunteer
            {
                Id = (int)v.Element("Id")!,
                FirstName = (string)v.Element("FirstName")!,
                LastName = (string)v.Element("LastName")!,
                Phone = (string)v.Element("Phone")!,
                Email = (string)v.Element("Email")!,
                Active = (bool)v.Element("IsActive")!,
                MyRole = (Role)Enum.Parse(typeof(Role), (string)v.Element("Role")!),
                Password = (string)v.Element("Password")!,
                Address = (string)v.Element("Address")!,
                Latitude = (double)v.Element("Latitude")!,
                Longitude = (double)v.Element("Longitude")!,
                LargestDistance = (double)v.Element("LargestDistance")!,
                MyDistanceType = (DistanceType)Enum.Parse(typeof(DistanceType),(string)v.Element("DistanceType")!)
            })
            .ToList();
        return filter == null ? volunteers : volunteers.Where(filter);
    }

    public void Update(Volunteer item)
    {
        XElement volunteersRootElem = XMLTools.LoadListFromXMLElement(Config.s_volunteers_xml);

        (volunteersRootElem.Elements().FirstOrDefault(v => (int?)v.Element("Id") == item.Id)
        ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={item.Id} does Not exist"))
                .Remove();

        volunteersRootElem.Add(new XElement("Volunteer", CreateVolunteerElement(item)));

        XMLTools.SaveListToXMLElement(volunteersRootElem, Config.s_volunteers_xml);
    }
}
