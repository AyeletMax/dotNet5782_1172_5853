using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Generic interface for CRUD operations on entities.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
namespace DalApi
{
    public interface ICrud<T> where T : class
    {
        void Create(T item);  // Create a new entity.
        T? Read(int id);      // Read an entity by ID.
        IEnumerable<T> ReadAll(Func<T, bool>? filter = null);  // Read all entities, with optional filter.
        void Update(T item);  // Update an existing entity.
        void Delete(int id);  // Delete an entity by ID.
        void DeleteAll();     // Delete all entities.
        T? Read(Func<T, bool> filter);  // Read the first entity matching a filter.
    }
}