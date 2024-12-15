namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Implements the ICall interface for managing 'Call' entities in XML.
/// Provides CRUD operations for Call entities.
/// </summary>
internal class CallImplementation : ICall
{
    /// Creates a new Call and adds it to the XML data source.
    public void Create(Call item)
    {
        int newId = Config.NextAssignmentId;
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        Call newCalls = item with { Id = newId };
        calls.Add(newCalls);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml); ;
    }

    /// Deletes a Call by its ID from the XML data source.
    public void Delete(int id)
    {
        List<Call> Calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (Calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Calls, Config.s_calls_xml);
    }

    /// Deletes all Calls from the XML data source.
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    /// Reads a Call by its ID from the XML data source.
    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);

        return calls.FirstOrDefault(item => item.Id == id);
    }

    /// Reads a Call by a filter from the XML data source.
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter);
    }

    /// Reads all Calls from the XML data source, optionally filtered.
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter == null
            ? calls
            : calls.Where(filter);
    }

    /// Updates an existing Call in the XML data source.
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does Not exist");
        calls.Add(item); ;
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }
 
}
