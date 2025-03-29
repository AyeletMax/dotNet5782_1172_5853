namespace BlApi;
// Static class responsible for creating and providing instances of IBl
public static class Factory
{
    // Static method that creates and returns an instance of BlImplementation.Bl,
    // which implements the IBl interface.
    // Returns:
    // An instance of IBl (BlImplementation.Bl).
    public static IBl Get() => new BlImplementation.Bl();
}
    

