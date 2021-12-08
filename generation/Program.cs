using System;
using static System.Console;
using consoleApp;
using Npgsql;
using System.Collections.Generic;
namespace generation
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc";
            CategoryRepository repo = new CategoryRepository(connString);
        }
    }
    public class ConsoleGenerate
    {
        private CategoryRepository cts_brs;
        private ItemRepository items;
        private ModRepository mods;
        public ConsoleGenerate(CategoryRepository cts_brs, ItemRepository items, ModRepository mods)
        {
            this.cts_brs = cts_brs;
            this.items = items;
            this.mods = mods;
        }
        public void InputRegistrateMod()
        {
            string name;
            while (true)
            {
                Write("Enter a unique name of moderator: ");
                name = ReadLine();
                if (Validation.CheckUnique(mods.GetUniqueNamesCount(name)))
                {
                    Moderator mod = new Moderator();
                    mod.name = name;
                    while (true)
                    {
                        Write("Enter a password: ");
                        string password = ReadLine();
                        Write("Enter this password again: ");
                        string password1 = ReadLine();
                        if (password == password1)
                        {
                            mod.password = password;
                            break;
                        }
                        else
                        {
                            WriteLine("Passwords are not the same, enter again!");
                        }
                    }
                    mods.Insert(mod);
                    break;
                }
                else
                {
                    WriteLine("Name isn`t unique, please enter again!");
                }
            }
        }
    }
    public class ConsoleLog
    {
        private Generation generation;
        public ConsoleLog(Generation generation)
        {
            this.generation = generation;
        }
        public void ProcessCommands(string connString)
        {
            while (true)
            {
                Write("Enter a command: ");
                string command = ReadLine();
                if (command.Contains("insert"))
                {
                    if (command.Contains("item"))
                    {
                        Item item = generation.FillItem();
                        WriteLine($"Id of new item is: {generation.items.Insert(item)}");
                    }
                    else
                    {
                        WriteLine("Unknown command.");
                    }
                }
                else if (command.Contains("generate"))
                {
                    int num;
                    while (true)
                    {
                        Write("Enter a num of generated values: ");
                        string value = ReadLine();
                        if (Validation.CheckInteger(value) && int.Parse(value) > 0)
                        {
                            num = int.Parse(value);
                            break;
                        }
                        else
                        {
                            WriteLine("Number wasn`t correct, please enter again!");
                        }
                    }
                    if(command.Contains("all"))
                    {
                        generation.GenerateItems(num);
                        generation.GenerateMods(num);
                    }
                    else if(command.Contains("items"))
                    {
                        generation.GenerateItems(num);
                    }
                    else if(command.Contains("mods"))
                    {
                        generation.GenerateMods(num);
                    }
                }
                else if (command == "exit" || command == "")
                {
                    WriteLine("Bye.");
                    break;
                }
                else
                {
                    WriteLine("Unknown command.");
                }
            }
        }
    }
    public class Generation
    {
        private NpgsqlConnection connection;
        public ItemRepository items;
        public CategoryRepository categories;
        public BrandRepository brands;
        public ModRepository mods;
        public Generation(string connString, ItemRepository items, CategoryRepository categories, BrandRepository brands, ModRepository mods)
        {
            this.connection = new NpgsqlConnection(connString);
            this.items = items;
            this.categories = categories;
            this.brands = brands;
            this.mods = mods;
        }
        public void GenerateItems(int num)
        {
            string[] names = GenerateStrings(num);
            double[] costs = GenerateDoubles(num);
            int[] brand_ids = GenerateBrands(num);
            int[] ctg_ids = GenerateCategories(num);
            for (int i = 0; i < num; i++)
            {
                Item item = new Item(names[i], costs[i], brand_ids[i], ctg_ids[i]);
                items.Insert(item);
            }
        }
        public int[] GenerateBrands(int num)
        {
            string[] names = GenerateStrings(num);
            int[] ids = new int[num];
            for (int i = 0; i < num; i++)
            {
                Brand brand = new Brand(names[i]);
                ids[i] = (int)brands.Insert(brand);
            }
            return ids;
        }
        public int[] GenerateCategories(int num)
        {
            string[] names = GenerateStrings(num);
            int[] ids = new int[num];
            for (int i = 0; i < num; i++)
            {
                Category category = new Category(names[i]);
                ids[i] = (int)categories.Insert(category);
            }
            return ids;
        }
        public string[] GenerateStrings(int num)
        {
            List<string> strings = new List<string>();
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"select chr(trunc(65+random()*25)::int) 
                || chr(trunc(65+random()*25)::int) || chr(trunc(65+random()*25)::int) 
                    || chr(trunc(65+random()*25)::int) from generate_series(1,@num)";
            command.Parameters.AddWithValue("num", num * 100);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                strings.Add(reader.GetString(0));
            }
            connection.Close();
            string[] stringRepo = new string[strings.Count];
            strings.CopyTo(stringRepo);
            return stringRepo;
        }
        public string[] GenerateModStrings(int num)
        {
            List<string> strings = new List<string>();
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"select chr(trunc(65+random()*25)::int) 
                || chr(trunc(65+random()*25)::int) || chr(trunc(65+random()*25)::int) 
                    || chr(trunc(65+random()*25)::int) from generate_series(1,@num)";
            command.Parameters.AddWithValue("num", num * 100);
            NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (Validation.CheckUnique(mods.GetUniqueNamesCount(reader.GetString(0))) && strings.Count != num)
                {
                    strings.Add(reader.GetString(0));
                }
                if (strings.Count == num)
                {
                    break;
                }
            }
            connection.Close();
            string[] stringRepo = new string[strings.Count];
            strings.CopyTo(stringRepo);
            return stringRepo;
        }
        public Item FillItem()
        {
            Item item = new Item();
            string costVal = "";
            Write("Enter a name of inserting item: ");
            item.name = ReadLine();
            while (true)
            {
                Write("Enter a cost of inserting item: ");
                costVal = ReadLine();
                if (Validation.CheckDouble(costVal))
                {
                    item.cost = double.Parse(costVal);
                    break;
                }
                else
                {
                    WriteLine("Cost is wrong, enter again!");
                }
            }
            while (true)
            {
                Write("Enter a name of category: ");
                string name = ReadLine();
                if (!Validation.CheckUnique(categories.GetUniqueNamesCount(name)))
                {
                    item.category_id = categories.GetByName(name).category_id;
                    break;
                }
                else
                {
                    WriteLine("Unknown category for item, please enter again!");
                }
            }
            while (true)
            {
                Write("Enter a name of brand: ");
                string name = ReadLine();
                if (!Validation.CheckUnique(brands.GetUniqueNamesCount(name)))
                {
                    item.brand_id = brands.GetByName(name).brand_id;
                    break;
                }
                else
                {
                    WriteLine("Unknown brand for item, please enter again!");
                }
            }
            return item;
        }
        public double[] GenerateDoubles(int num)
        {
            List<double> doubles = new List<double>();
            connection.Open();
            NpgsqlCommand command1 = connection.CreateCommand();
            command1.CommandText = "select trunc(random()*100000)::int from generate_series(1,@num)";
            command1.Parameters.AddWithValue("num", num);
            NpgsqlDataReader reader1 = command1.ExecuteReader();
            while (reader1.Read())
            {
                doubles.Add(reader1.GetInt32(0));
            }
            connection.Close();
            double[] doubRepo = new double[doubles.Count];
            doubles.CopyTo(doubRepo);
            return doubRepo;
        }
        public int[] GenerateMods(int num)
        {
            int[] array = new int[num];
            string[] names = GenerateModStrings(num);
            string[] passwords = GenerateStrings(num);
            for (int i = 0; i < num; i++)
            {
                Moderator mod = new Moderator(names[i], passwords[i]);
                array[i] = (int)mods.Insert(mod);
            }
            return array;
        }
    }
}
