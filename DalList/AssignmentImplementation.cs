namespace Dal;
using DalApi;
using DO;
using System;
using System.Linq;
/// <summary>
/// Implements CRUD operations for Assignment entities.
/// </summary>

internal class AssignmentImplementation : IAssignment
{
    /// <summary>Creates a new Assignment and adds it to the data source.</summary>
    public void Create(Assignment item)
    {
        int newId = Config.NextAssignmentId;
        Assignment newAssignment = item with { Id = newId };
        DataSource.Assignments.Add(newAssignment);
    }

    /// <summary>Throws an exception that Assignments cannot be deleted.</summary>
    public void Delete(int id)
    {
        throw new DalDeletionImpossible("Assignments cannot be deleted.");
    }

    /// <summary>Throws an exception that Assignments cannot be deleted</summary>
    public void DeleteAll()
    {
        throw new DalDeletionImpossible("Assignments cannot be deleted.");
    }
    /// <summary>Reads an Assignment by ID.</summary>
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id);
    }  
    /// <summary>Reads all Assignments, optionally filtered.</summary>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
  => filter == null
      ? DataSource.Assignments.Select(item => item)
      : DataSource.Assignments.Where(filter);

    /// <summary>Updates an existing Assignment. Throws DalDoesNotExistException if not found.</summary>
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
            throw new DalDoesNotExistException($"Could not Update Item, no assignment with Id{item.Id} found");
        }
    }
    /// <summary>Reads the first Assignment that matches a filter.</summary>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }
}
