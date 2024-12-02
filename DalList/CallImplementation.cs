using DalApi;

namespace Dal;
using DalApi;
using DO;
//using System.Collections.Generic;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int newId = Config.NextCallId;
        Call call=item with { Id = newId };
        DataSource.Calls.Add(call);
    }

    public void Delete(int id)
    {
        Call? call = Read(id);
        if (call != null)
        {
            DataSource.Calls.Remove(call);
        }
        else
        {
            throw new DalDeletionImpossible($"Call with Id{id} was not found");

        }
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(item => item.Id == id);
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
   => filter == null
       ? DataSource.Calls.Select(item => item)
       : DataSource.Calls.Where(filter);

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
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
}
