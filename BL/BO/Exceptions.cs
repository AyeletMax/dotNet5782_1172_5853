namespace BO;


 public class BLAlreadyExistsException : Exception
 {
     public BLAlreadyExistsException(string? message) : base(message) { }
 }
public class GeneralDatabaseException : Exception
{
    public GeneralDatabaseException(string? message, Exception? innerException) : base(message) { }
}
public class BLDoesNotExistException : Exception
{
    public BLDoesNotExistException(string? message, Exception ex) : base(message) { }
}

public class GeolocationNotFoundException : Exception
{
    public GeolocationNotFoundException(string? message) : base(message) { }
}
public class BlDoesNotExistn : Exception
{
    public BlDoesNotExistn(string? message) : base(message) { }
}
public class InvalidFormatException : Exception
{
    public InvalidFormatException(string? message) : base(message) { }
}
public class DeletionException : Exception
{
    public DeletionException(string? message) : base(message) { }
}
public class BLDoesNotExist : Exception
{
    public BLDoesNotExist(string? message, Exception ex) : base(message) { }    
}
public class AssignmentNotFoundException : Exception
{
    public AssignmentNotFoundException(string? message) : base(message) { }    
}
public class PermissionDeniedException : Exception
{
    public PermissionDeniedException(string? message) : base(message)    { }
}
public class InvalidOperationException : Exception
{
    public InvalidOperationException(string? message) : base(message)    { }
}
