namespace BO;



public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }

    public BlAlreadyExistsException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }

    public BlDoesNotExistException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
public class BlGeneralDatabaseException : Exception
{
    public BlGeneralDatabaseException(string? message, Exception? innerException) : base(message) { }
}
public class BlGeolocationNotFoundException : Exception
{
    public BlGeolocationNotFoundException(string? message) : base(message) { }
}
public class BlInvalidFormatException : Exception
{
    public BlInvalidFormatException(string? message) : base(message) { }
}
public class BlDeletionException : Exception
{
    public BlDeletionException(string? message) : base(message) { }
}

public class BlApiRequestException : Exception
{
    public BlApiRequestException(string? message) : base(message) { }
}

public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message)    { }
}
public class BlInvalidOperationException : Exception
{
    public BlInvalidOperationException(string? message) : base(message) { }

    public BlInvalidOperationException(string? message, Exception? innerException)
        : base(message, innerException) { }
}
//public class AssignmentNotFoundException : Exception
//{
//    public AssignmentNotFoundException(string? message) : base(message) { }    
//}
//public class InvalidAddressException : Exception
//{
//    public InvalidAddressException(string? message) : base(message) { }
//}
