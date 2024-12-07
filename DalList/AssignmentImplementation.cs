namespace Dal;
using DalApi;
using DO;
using System;
using System.Linq;
/// <summary>
/// all CRUD functions for Assignment
/// </summary>

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int newId = Config.NextAssignmentId;
        Assignment newAssignment = item with { Id = newId };
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
            throw new DalDoesNotExistException($"Assignment with Id{id} was found");
        }
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(item => item.Id == id);
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
  => filter == null
      ? DataSource.Assignments.Select(item => item)
      : DataSource.Assignments.Where(filter);

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
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }
}
