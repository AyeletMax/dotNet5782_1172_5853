namespace BO;


/// <summary>
/// Custom exception classes for the business logic layer.
/// </summary>
 

/// <summary>
/// Represents an exception that occurs when an entity already exists in the business logic layer.
/// </summary>
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string? message) : base(message) { }

    public BlAlreadyExistsException(string? message, Exception? innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Represents an exception that occurs when an entity does not exist in the business logic layer.
/// </summary>
public class BlDoesNotExistException : Exception
{
    public BlDoesNotExistException(string? message) : base(message) { }

    public BlDoesNotExistException(string? message, Exception? innerException)
        : base(message, innerException) { }
}

/// <summary>
/// Represents a general exception that occurs during database operations in the business logic layer.
/// </summary>
public class BlGeneralDatabaseException : Exception
{
    public BlGeneralDatabaseException(string? message, Exception? innerException) : base(message) { }
}

/// <summary>
/// Represents an exception that occurs when geolocation data is not found in the business logic layer.
/// </summary>
public class BlGeolocationNotFoundException : Exception
{
    public BlGeolocationNotFoundException(string? message) : base(message) { }
}

/// <summary>
/// Represents an exception that occurs when there is an invalid format in the business logic layer.
/// </summary>
public class BlInvalidFormatException : Exception
{
    public BlInvalidFormatException(string? message) : base(message) { }
}

/// <summary>
/// Represents an exception that occurs during the deletion process in the business logic layer.
/// </summary>
public class BlDeletionException : Exception
{
    public BlDeletionException(string? message) : base(message) { }
}

/// <summary>
/// Represents an exception that occurs during an API request in the business logic layer.
/// </summary>
public class BlApiRequestException : Exception
{
    public BlApiRequestException(string? message) : base(message) { }
}

// <summary>
/// Represents an exception that occurs due to unauthorized access in the business logic layer.
/// </summary>
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string? message) : base(message)    { }
}
/// <summary>
/// Represents an exception that occurs during an invalid operation in the business logic layer.
/// </summary>
public class BlInvalidOperationException : Exception
{
    public BlInvalidOperationException(string? message) : base(message) { }

    public BlInvalidOperationException(string? message, Exception? innerException)
        : base(message, innerException) { }
}

