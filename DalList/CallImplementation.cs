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
            throw new Exception($"Call with Id{id} was found");
        }
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        return DataSource.Calls.Find(a => a.Id == id);
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }
   
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
            throw new Exception($"Could not Update Item, no Call with Id{item.Id} found");
        }
    }
}
