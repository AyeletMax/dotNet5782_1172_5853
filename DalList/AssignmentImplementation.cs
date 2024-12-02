namespace Dal;
using DalApi;
using DO;
using System;

//using System.Collections.Generic;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int newId = Config.NextAssignmentId;
        Assignment newAssignment = item with { Id=newId};
        DataSource.Assignments.Add(newAssignment);
    }

    public void Delete(int id)
    {
        Assignment? assignment = Read(id);
        if (assignment != null)
        {
            DataSource.Assignments.Remove(assignment);
        }
        else
        {
            throw new Exception($"Assignment with Id{id} was found");
        }
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.Find(a => a.Id == id);
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        Assignment? existingAssignment = Read(item.Id);
        if (existingAssignment != null)
        {
            DataSource.Assignments.Remove(existingAssignment);
            DataSource.Assignments.Add(item);
        }
        else
        {
            throw new Exception($"Could not Update Item, no assignment with Id{item.Id} found");

        }

    }
}
