using System;
namespace targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome1172();
            Welcome5853();
            Console.ReadKey();
        }

        static partial void Welcome5853();
        private static void Welcome1172()
        {
            Console.WriteLine("Enter your name: ");
            string name = Console.ReadLine();
            Console.WriteLine($"{name}, welcome to my first console application");
        }
    }
}