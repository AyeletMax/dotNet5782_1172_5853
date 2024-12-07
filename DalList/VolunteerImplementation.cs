namespace Dal;
using DalApi;
using DO;

internal class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
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
            throw new DalDoesNotExistException($"Volunteer with Id {id} was not found");

        }
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(item => item.Id == id);
    }


    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) 
     => filter == null
         ? DataSource.Volunteers.Select(item => item)
         : DataSource.Volunteers.Where(filter);

  


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
            throw new DalDoesNotExistException($"Could not Update Item, no Volunteer with Id{item.Id} found");

        }
    }
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}
