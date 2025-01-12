namespace BO;


 public class DalAlreadyExistsException : Exception
 {
     public DalAlreadyExistsException(string? message) : base(message) { }
 }

