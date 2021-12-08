using System;
using static System.Console;
using System.Collections.Generic;
namespace consoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc";
            CategoryRepository repo = new CategoryRepository(connString);
            WriteLine("Hello from console");
        }
    }
    public static class Validation
    {
        static string connString = "Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc";
        static CategoryRepository cts_brs;
        static ItemRepository items;
        static ModRepository mods;
        static Validation()
        {
            cts_brs = new CategoryRepository(connString);
            items = new ItemRepository(connString);
            mods = new ModRepository(connString);
        }
        public static bool CheckInteger(string value)
        {
            return int.TryParse(value, out int num);
        }
        public static bool CheckDouble(string value)
        {
            return double.TryParse(value, out double num);
        }
        public static int GetInt()
        {
            int num;
            while (true)
            {
                WriteLine("Enter a num: ");
                string value = ReadLine();
                if (int.TryParse(value, out num))
                {
                    num = int.Parse(value);
                    break;
                }
                else
                {
                    WriteLine("Wrong input, enter again");
                }
            }
            return num;
        }
        public static bool CheckUnique(long num)
        {
            return num == 0;
        }
    }
}
