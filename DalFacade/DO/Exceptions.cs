namespace DO;
[Serializable]
/// <summary>
/// Exception thrown when a requested DAL entity does not exist.
/// </summary>
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) {
       
    }
}
/// <summary>
/// Exception thrown when an entity already exists in the DAL.
/// </summary>
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
/// <summary>
/// Exception thrown when deletion of an entity is not possible.
/// </summary>
public class DalDeletionImpossible : Exception
{
    public DalDeletionImpossible(string? message) : base(message) { }
}
/// <summary>
/// Custom exception class for errors that occur while loading or creating a DAL XML file.
/// </summary>
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}
