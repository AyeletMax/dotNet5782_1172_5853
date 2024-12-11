namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int newId = Config.NextAssignmentId;
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call newAssignment = item with { Id = newId };
        calls.Add(newAssignment);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); ;
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public void DeleteAll()
    {
        throw new NotImplementedException();
    }

    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        return assignments.FirstOrDefault(item => item.Id == id);
    }

    public Call? Read(Func<Call, bool> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        throw new NotImplementedException();
    }

    public void Update(Call item)
    {
        throw new NotImplementedException();
    }
}
