namespace DalTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //לא הוספנו פה לוגיקה
                Console.WriteLine("Hello, World!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}