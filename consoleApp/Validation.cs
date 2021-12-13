using static System.Console;
namespace consoleApp
{
    public static class Validation
    {
        static string connString = "Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc";
        static courseWorkdbContext context = new courseWorkdbContext();
        static CategoryRepository categories;
        static BrandRepository brands;
        static ItemRepository items;
        static ModRepository mods;
        static Validation()
        {
            categories = new CategoryRepository(connString, context);
            items = new ItemRepository(connString, context);
            mods = new ModRepository(connString, context);
            brands = new BrandRepository(connString, context);
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
