namespace DO;
[Serializable]//למה צריך את זה?
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) {
       
    }
}
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}


