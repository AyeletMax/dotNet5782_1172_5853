namespace Dal;
using DalApi;
using DO;
//using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        var assignment = Read(id);
        if (assignment != null)
        {
            DataSource.Volunteers.Remove(assignment);
        }
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.Find(a => a.Id == id);
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        var existingAssignment = Read(item.Id);
        if (existingAssignment != null)
        {
            DataSource.Volunteers.Remove(existingAssignment);
            DataSource.Volunteers.Add(item);
        }
    }
}
