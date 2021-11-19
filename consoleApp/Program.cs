using System;
using static System.Console;
namespace consoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc";
            Category_Brend_Repo repo = new Category_Brend_Repo(connString);
            WriteLine("Hello from console");
        }
    }
    public class Validation
    {
        private ItemRepository items;
        private ModRepository mods;
        private Category_Brend_Repo cts_brs;

        public Validation(ItemRepository items, ModRepository mods, Category_Brend_Repo cts_brs)
        {
            this.items = items;
            this.mods = mods;
            this.cts_brs = cts_brs;
        }
        public int GetInt()
        {
            int num;
            while(true)
            {
                WriteLine("Enter a num: ");
                string value = ReadLine();
                if(int.TryParse(value, out num))
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
        
    }
    public class Generation
    {

    }
}
