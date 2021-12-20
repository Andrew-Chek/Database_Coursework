using static System.Console;
using Npgsql;
using System.Collections.Generic;
using System.IO;
using RepoCode;
using PredictionLib;
namespace generation
{
    public class Generation
    {
        private NpgsqlConnection connection;
        public ItemRepository items;
        public CategoryRepository categories;
        public BrandRepository brands;
        public ModRepository mods;
        public CostPrediction prediction;
        public Generation(string connString, ItemRepository items, CategoryRepository categories, BrandRepository brands, ModRepository mods, CostPrediction prediction)
        {
            this.connection = new NpgsqlConnection(connString);
            this.items = items;
            this.categories = categories;
            this.brands = brands;
            this.mods = mods;
            this.prediction = prediction;
        }
        public void GenerateItems(int num)
        {
            string[] names = GenerateStrings(num);
            double[] costs = GenerateDoubles(num);
            int[] brand_ids = GenerateBrands(num);
            int[] ctg_ids = GenerateCategories(num);
            int[] createYears = GenerateYears(num);
            for (int i = 0; i < num; i++)
            {
                Item item = new Item(names[i], costs[i], brand_ids[i], ctg_ids[i], createYears[i]);
                items.Insert(item);
            }
        }
        public void GenerateCSVItems(int num)
        {
            string[] names = GenerateStrings(num);
            string[] brands = GenerateStrings(num);
            double[] costs = GenerateDoubles(num);
            string[] ctgs = GenerateStrings(num);
            int[] createYears = GenerateYears(num);
            StreamWriter writer = new StreamWriter("./items_prediction.csv");
            for (int i = 0; i < num; i++)
            {
                ItemForPrediction item = new ItemForPrediction(i, names[i], (float)costs[i], brands[i], ctgs[i], createYears[i]);
                string item_value = item.ToString();
                writer.Write(item_value);
                if(i != num-1)
                {
                    writer.Write("\r\n");
                }
            }
            writer.Close();
        }
        public void GetCSVItemsFromDB()
        {
            List<Item> allItems = items.GetAll();
            Item[] itemArray = new Item[allItems.Count];
            allItems.CopyTo(itemArray);
            StreamWriter writer = new StreamWriter("./items_prediction.csv");
            for (int i = 0; i < allItems.Count; i++)
            {
                ItemForPrediction item = prediction.ItemToItemForPrediction(itemArray[i], categories, brands);
                string item_value = item.ToString();
                writer.Write(item_value);
                if(i != allItems.Count - 1)
                {
                    writer.Write("\r\n");
                }
            }
            writer.Close();
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
            List<string> oldBrandNames = brands.GetAllNames();
            List<string> oldCtgNames = categories.GetAllNames();
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
                        Item item = new Item();
                        item.name = array[0];
                        item.cost = double.Parse(array[1]);
                        if(!oldBrandNames.Contains(array[2]))
                        {
                            Brand brand = new Brand();
                            brand.brand = array[2];
                            item.brand_id = (int)brands.Insert(brand);
                            oldBrandNames.Add(brand.brand);
                        }
                        else
                            item.brand_id = brands.GetByName(array[2]).brand_id;
                        if(!oldCtgNames.Contains(array[3]))
                        {
                            Category category = new Category();
                            category.category = array[3];
                            item.category_id = (int)categories.Insert(category);
                            oldCtgNames.Add(category.category);
                        }
                        else
                            item.category_id = categories.GetByName(array[3]).category_id;
                        item.createYear = int.Parse(array[4]);
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
                if (mods.GetUniqueNamesCount(reader.GetString(0)) != 0 && strings.Count != num)
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
                if (double.TryParse(costVal, out item.cost))
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
            if (categories.GetUniqueNamesCount(ctg) != 0)
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
            if (brands.GetUniqueNamesCount(brand_name) != 0)
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
        public int[] GenerateYears(int num)
        {
            List<int> years = new List<int>();
            connection.Open();
            NpgsqlCommand command1 = connection.CreateCommand();
            command1.CommandText = "select trunc(random()* (2022-1980 + 1) + 1980)::int from generate_series(1,@num);";
            command1.Parameters.AddWithValue("num", num);
            NpgsqlDataReader reader1 = command1.ExecuteReader();
            while (reader1.Read())
            {
                years.Add(reader1.GetInt32(0));
            }
            connection.Close();
            int[] yearRepo = new int[years.Count];
            years.CopyTo(yearRepo);
            return yearRepo;
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
