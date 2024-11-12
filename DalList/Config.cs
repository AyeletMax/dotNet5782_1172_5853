namespace Dal;

internal static class Config
{
    internal const int StartId = 1;
    private static int id = StartId;
    internal static int Id { get => id++; }
    //...

    internal static DateTime Clock { get; set; } = DateTime.Now;
    //...

    internal static void Reset()
    {
        nextCallId = startCourseId;
        Clock = DateTime.Now;
       
        //...
        Clock = DateTime.Now;
        //...
    }
}

