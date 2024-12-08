namespace Dal;
using DalApi;
using DO;

/// Provides all CRUD operations for the `Volunteer` entity.
internal class VolunteerImplementation : IVolunteer
{
    /// Creates a new volunteer.
    /// <param name="item">The volunteer to be added.</param>
    /// <exception cref="DalAlreadyExistsException">Thrown if a volunteer with the same ID already exists.</exception>
    public void Create(Volunteer item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        DataSource.Volunteers.Add(item);
    }

    /// Deletes a volunteer by ID.
    /// <param name="id">The ID of the volunteer to be deleted.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no volunteer with the specified ID is found.</exception>
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

    /// Deletes all volunteers.
    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// Reads a volunteer by ID.
    /// <param name="id">The ID of the volunteer to read.</param>
    /// <returns>The volunteer with the specified ID, or null if not found.</returns>
    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(item => item.Id == id);
    }

    /// Reads all volunteers, optionally filtering them.
    /// <param name="filter">A predicate to filter the volunteers.</param>
    /// <returns>A list of volunteers that match the filter, or all volunteers if no filter is provided.</returns>
    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
    {
        return filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);
    }

    /// Updates an existing volunteer.
    /// <param name="item">The updated volunteer data.</param>
    /// <exception cref="DalDoesNotExistException">Thrown if no volunteer with the specified ID is found.</exception>
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
            throw new DalDoesNotExistException($"Could not Update Item, no Volunteer with Id {item.Id} found");
        }
    }

    /// Reads a volunteer that matches the provided filter.
    /// <param name="filter">A predicate to filter the volunteers.</param>
    /// <returns>The first volunteer that matches the filter, or null if none found.</returns>
    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}
