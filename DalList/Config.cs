namespace Dal;
internal static class Config
{
    internal const int startId = 1;
    private static int id = startId;
    internal static int Id { get => id++; }
    //...

    internal static DateTime Clock { get; set; } = DateTime.Now;
    //...

    internal static void Reset()
    {
        nextCallId = startCourseId;
        Clock = DateTime.Now;
       
    }
}

