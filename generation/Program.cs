using System;
using static System.Console;
using consoleApp;
using Npgsql;
using System.Collections.Generic;
using System.IO;
namespace generation
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc";
            courseWorkdbContext context = new courseWorkdbContext();
            CategoryRepository ctgs = new CategoryRepository(connString, context);
            BrandRepository brands = new BrandRepository(connString, context);
            ItemRepository items = new ItemRepository(connString, context);
            ModRepository mods = new ModRepository(connString, context);
            Generation generation = new Generation(connString, items, ctgs, brands, mods);
            ConsoleLog console = new ConsoleLog(generation);
            console.ProcessCommands();
        }
    }
    /*public class ConsoleGenerate
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
    }*/
    public class ConsoleLog
    {
        private Generation generation;
        public ConsoleLog(Generation generation)
        {
            this.generation = generation;
        }
        public void ProcessCommands()
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
                    else if(command.Contains("dataset"))
                    {
                        generation.GenerateModsDataset(num);
                        generation.GenerateItemsDataset(num);
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
        public void GenerateModsDataset(int num)
        {
            if(num > 50)
            {
                WriteLine("Dataset doesn`t have so many values, try with less number");
                return;
            }
            string filePath = "./mods.csv";
            int count = 0;
            int realCount = 0;
            List<string> oldModNames = mods.GetAllNames();
            for(int i = 0; i < num; i++)
            {
                if(count == num)
                {
                    break;
                }
                StreamReader reader = new StreamReader(filePath);
                string s = "";
                while(true)
                {
                    s = reader.ReadLine();
                    if (s == null)
                    {
                        break;
                    }
                    if(s.StartsWith("name"))
                    {
                        continue;
                    }
                    else
                    {
                        string[] array = s.Split(",");
                        if(!oldModNames.Contains(array[0]))
                        {
                            Moderator mod = new Moderator(array[0], array[1]);
                            mods.Insert(mod);
                            realCount++;
                        }
                        count++;
                    }
                    if(count == num)
                    {
                        break;
                    }
                }
                if(count == num)
                {
                    break;
                }
            }
            WriteLine($"The real number of unique mods added: {realCount}");
        }
        public void GenerateItemsDataset(int num)
        {
            if(num > 50)
            {
                WriteLine("Dataset doesn`t have so many values, try with less number");
                return;
            }
            string filePath = "./items.csv";
            int count = 0;
            for(int i = 0; i < num; i++)
            {
                List<string> oldBrandNames = brands.GetAllNames();
                List<string> oldCtgNames = categories.GetAllNames();
                if(count == num)
                {
                    break;
                }
                StreamReader reader = new StreamReader(filePath);
                string s = "";
                while(true)
                {
                    s = reader.ReadLine();
                    if (s == null)
                    {
                        break;
                    }
                    if(s.StartsWith("name"))
                    {
                        continue;
                    }
                    else
                    {
                        string[] array = s.Split(",");
                        Item item = new Item();
                        item.name = array[0];
                        item.cost = double.Parse(array[1]);
                        if(!oldBrandNames.Contains(array[2]))
                        {
                            Brand brand = new Brand();
                            brand.brand = array[2];
                            item.brand_id = (int)brands.Insert(brand);
                        }
                        else
                            item.brand_id = brands.GetByName(array[2]).brand_id;
                        if(!oldCtgNames.Contains(array[3]))
                        {
                            Category category = new Category();
                            category.category = array[3];
                            item.category_id = (int)categories.Insert(category);
                        }
                        else
                            item.category_id = categories.GetByName(array[3]).category_id;
                        items.Insert(item);
                        count++;
                    }
                    if(count == num)
                    {
                        break;
                    }
                }
                if(count == num)
                {
                    break;
                }
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
            Write("Enter a name of category: ");
            string ctg = ReadLine();
            if (!Validation.CheckUnique(categories.GetUniqueNamesCount(ctg)))
            {
                item.category_id = categories.GetByName(ctg).category_id;
            }
            else
            {
                Category category = new Category(ctg);
                item.category_id = (int)categories.Insert(category);
            }
            Write("Enter a name of brand: ");
            string brand_name = ReadLine();
            if (!Validation.CheckUnique(brands.GetUniqueNamesCount(brand_name)))
            {
                item.brand_id = brands.GetByName(brand_name).brand_id;
            }
            else
            {
                Brand brand = new Brand(brand_name);
                item.brand_id = (int)brands.Insert(brand);
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
