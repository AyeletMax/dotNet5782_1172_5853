using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// Generic CRUD operations for entities.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
namespace DalApi
{
    public interface ICrud<T> where T : class
    {
        /// <summary>Create a new entity.</summary>
        void Create(T item);

        /// <summary>Read an entity by ID.</summary>
        T? Read(int id);

        /// <summary>Read all entities, with optional filter.</summary>
        IEnumerable<T> ReadAll(Func<T, bool>? filter = null);

        /// <summary>Update an existing entity.</summary>
        void Update(T item);

        /// <summary>Delete an entity by ID.</summary>
        void Delete(int id);

        /// <summary>Delete all entities.</summary>
        void DeleteAll();

        /// <summary>Read the first entity matching a filter.</summary>
        T? Read(Func<T, bool> filter);
    }
}