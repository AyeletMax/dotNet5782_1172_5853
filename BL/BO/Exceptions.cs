namespace BO;


 public class DalAlreadyExistsException : Exception
 {
     public DalAlreadyExistsException(string? message) : base(message) { }
 }
public class GeneralDatabaseException : Exception
{
    public GeneralDatabaseException(string? message, Exception? innerException) : base(message) { }
}
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message)
    {

    }
}

