using DalApi;

namespace Dal;
using DalApi;
using DO;
//using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        DataSource.Calls.Add(item);
    }

    public void Delete(int id)
    {
        var call = Read(id);
        if (call != null)
        {
            DataSource.Calls.Remove(call);
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
        var existingCall = Read(item.Id);
        if (existingCall != null)
        {
            DataSource.Calls.Remove(existingCall);
            DataSource.Calls.Add(item);
        }
    }
}
