namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

/// <summary>
/// Implements the IAssignment interface for managing 'Assignment' entities in XML.
/// Provides CRUD operations for Assignment entities.
/// </summary>
internal class AssignmentImplementation : IAssignment
{
    /// Creates a new Assignment and adds it to the XML data source.
    public void Create(Assignment item)
    {
        int newId = Config.NextAssignmentId;
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        Assignment newAssignment = item with { Id = newId };
        assignments.Add(newAssignment);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }

    //צריך לשאול מישהי על הזריקות פה
    /// Deletes an Assignment by its ID from the XML data source.
    public void Delete(int id)
    {
        List<Assignment> Assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (Assignments.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={id} does Not exist");
        XMLTools.SaveListToXMLSerializer(Assignments, Config.s_assignments_xml);
    }

    /// Deletes all Assignments from the XML data source.
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Assignment>(), Config.s_assignments_xml);

    }

    /// Reads an Assignment by its ID from the XML data source.
    public Assignment? Read(int id)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);

        return assignments.FirstOrDefault(item => item.Id == id);
    }

    /// Reads an Assignment by a filter from the XML data source.
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return assignments.FirstOrDefault(filter);
    }

    /// Reads all Assignments from the XML data source, optionally filtered.
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        return filter == null
            ? assignments
            : assignments.Where(filter);
    }

    /// Updates an existing Assignment in the XML data source.
    public void Update(Assignment item)
    {
        List<Assignment> assignments = XMLTools.LoadListFromXMLSerializer<Assignment>(Config.s_assignments_xml);
        if (assignments.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist");
        assignments.Add(item);
        XMLTools.SaveListToXMLSerializer(assignments, Config.s_assignments_xml);
    }
}
