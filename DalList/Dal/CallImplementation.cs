namespace Dal;
using DalApi;
using DO;
/// <summary>
/// Implements CRUD operations for Call entities.
/// </summary>

internal class CallImplementation : ICall
{
    /// <summary>Creates a new Call and adds it to the data source.</summary>
    public void Create(Call item)
    {
        int newId = Config.NextCallId;
        Call call=item with { Id = newId };
        DataSource.Calls.Add(call);
    }

    /// <summary>Deletes a Call by ID. Throws DalDoesNotExistException if not found.</summary>
    public void Delete(int id)
    {
        Call? call = Read(id);
        if (call != null)
        {
            DataSource.Calls.Remove(call);
        }
        else
        {
            throw new DalDoesNotExistException($"Call with Id{id} was not found");

        }
    }
    /// <summary>Deletes all Calls from the data source.</summary>
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }
    /// <summary>Reads a Call by ID.</summary>
    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item.Id == id);
    }
    /// <summary>Reads all Calls, optionally filtered.</summary>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
   => filter == null
       ? DataSource.Calls.Select(item => item)
       : DataSource.Calls.Where(filter);
    /// <summary>Updates an existing Call. Throws DalDoesNotExistException if not found.</summary>
    public void Update(Call item)
    {
        Call? existingCall = Read(item.Id);
        if (existingCall != null)
        {
            DataSource.Calls.Remove(existingCall);
            DataSource.Calls.Add(item);
        }
        else
        {
            throw new DalDoesNotExistException($"Could not Update Item, no Call with Id{item.Id} found");
        }
    }

    /// <summary>Reads the first Call that matches a filter.</summary>
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
}
