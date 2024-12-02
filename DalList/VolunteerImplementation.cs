namespace Dal;
using DalApi;
using DO;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new Exception($"Volunteer with ID={item.Id} already exists");
        DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        Volunteer? volunteer = Read(id);
        if (volunteer != null)
        {
            DataSource.Volunteers.Remove(volunteer);
        }
        else
        {
            throw new Exception($"Volunteer with Id{id} was found");
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
        Volunteer? volunteer = Read(item.Id);
        if (volunteer != null)
        {
            DataSource.Volunteers.Remove(volunteer);
            DataSource.Volunteers.Add(item);
        }
        else
        {
            throw new Exception($"Could not Update Item, no Volunteer with Id{item.Id} found");

        }
    }
}
