namespace BO;


 public class DalAlreadyExistsException : Exception
 {
     public DalAlreadyExistsException(string? message) : base(message) { }
 }
public class GeneralDatabaseException : Exception
{
    public GeneralDatabaseException(string? message, Exception? innerException) : base(message) { }
}
public class BLDoesNotExistException : Exception
{
    public BLDoesNotExistException(string? message, Exception ex) : base(message)
    {

    }
}

public class GeolocationNotFoundException : Exception
{
    public GeolocationNotFoundException(string? message) : base(message)
    {

    }
}
public class BlDoesNotExistn : Exception
{
    public BlDoesNotExistn(string? message) : base(message)
    {

    }
}
public class InvalidFormatException : Exception
{
    public InvalidFormatException(string? message) : base(message)
    {

    }
}

