using static System.Console;
using System.Diagnostics;
using Microsoft.ML;
using RepoCode;
using PredictionLib;
using generation;
namespace consoleApp
{
    public class ConsoleLog
    {
        private ItemRepository items;
        private CategoryRepository ctgs;
        private BrandRepository brands;
        private ModRepository mods;
        private CostPrediction prediction;
        private Autentification autentification;
        private Generation generation = new Generation("Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc");

        public ConsoleLog(ItemRepository items, CategoryRepository ctgs, BrandRepository brands, ModRepository mods, CostPrediction prediction, Autentification autentification)
        {
            this.items = items;
            this.ctgs = ctgs;
            this.brands = brands;
            this.mods = mods;
            this.prediction = prediction;
            this.autentification = autentification;
        }

        public string GetId(string command, int number)
        {
            string id_value = "";
            for (int i = number; i < command.Length; i++)
            {
                id_value += command[i];
            }
            return id_value;
        }
        public int[] GetMeasures(string value)
        {
            int[] arr = new int[2];
            string[] array = value.Split(',');
            if (array.Length != 2)
            {
                return arr;
            }
            if (int.TryParse(array[0], out arr[0]) && int.TryParse(array[1], out arr[1])
                && int.Parse(array[1]) > int.Parse(array[0]))
            {
                arr[0] = int.Parse(array[0]);
                arr[1] = int.Parse(array[1]);
            }
            return arr;
        }
        public void ProcessCommands()
        {
            mods.DroppingIndexes();
            items.DroppingIndexes();
            ctgs.DroppingIndexes();
            brands.DroppingIndexes();
            bool indexCheck = false;
            double num1 = 0;
            double num2 = 0;
            WriteLine("Now program without indexes");
            while(true)
            {
                Write("Enter a command: ");
                string command = ReadLine();
                if (command.Contains("get"))
                {
                    string numVal = GetId(command, 5);
                    int id;
                    if (int.TryParse(numVal, out id))
                    {
                        id = int.Parse(numVal);
                        if (command[4] == 'i')
                        {
                            ProcessGetItem(id);
                        }
                        else if (command[4] == 'c')
                        {
                            ProcessGetCategory(id);
                        }
                        else if (command[4] == 'm')
                        {
                            ProcessGetMod(id);
                        }
                        else if (command[4] == 'b')
                        {
                            ProcessGetBrand(id);
                        }
                        else
                        {
                            WriteLine("Unknown command.");
                        }
                    }
                    else
                    {
                        WriteLine("Id should be a number");
                    }
                }
                else if (command.Contains("delete"))
                {
                    string numVal = GetId(command, 7);
                    if (int.TryParse(numVal, out int num))
                    {
                        int id = int.Parse(numVal);
                        if (command[7] == 'i')
                        {
                            ProcessDelItem(id);
                        }
                        else if (command[7] == 'c')
                        {
                            ProcessDelCategory(id);
                        }
                        else if (command[7] == 'b')
                        {
                            ProcessDelBrand(id);
                        }
                        else if (command[7] == 'm')
                        {
                            ProcessDelMod(id);
                        }
                        else
                        {
                            WriteLine("Unknown command.");
                        }
                    }
                    else
                    {
                        WriteLine("Id should be a number");
                    }
                }
                else if (command.Contains("update"))
                {
                    string numVal = GetId(command, 8);
                    if (int.TryParse(numVal, out int num))
                    {
                        int id = int.Parse(numVal);
                        if (command[7] == 'i')
                        {
                            Item item = items.GetById(id);
                            Item newItem = SetItem(item);
                            WriteLine($"Was item updated successfully? - {items.Update(newItem)}");
                        }
                        else if (command[7] == 'c')
                        {
                            Category category = ctgs.GetById(id);
                            Category newCategory = SetCategory(category);
                            WriteLine($"Was order updated successfully? - {ctgs.Update(newCategory)}");
                        }
                        else if (command[7] == 'b')
                        {
                            Brand brand = brands.GetById(id);
                            Brand newBrand = SetBrand(brand);
                            WriteLine($"Was order updated successfully? - {brands.Update(newBrand)}");
                        }
                        else if (command[7] == 'm')
                        {
                            Moderator mod = mods.GetById(id);
                            Moderator newMod = FillMod();
                            WriteLine($"Was user updated successfully? - {mods.Update(newMod)}");
                        }
                        else
                        {
                            WriteLine("Unknown command.");
                        }
                    }
                    else
                    {
                        WriteLine("Id should be a number");
                    }
                }
                else if (command.Contains("search"))
                {
                    Write("Enter a search subline: ");
                    string value = ReadLine();
                    if (command.Contains("items"))
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        ProcessSearchItems(value);
                        sw.Stop();
                        if(indexCheck)
                        {
                            num1 = sw.ElapsedMilliseconds;
                        }
                        else
                            num2 = sw.ElapsedMilliseconds;
                    }
                    else if (command.Contains("categories"))
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        ProcessSearchCategories(value);
                        sw.Stop();
                        if(!indexCheck)
                        {
                            num1 = sw.ElapsedMilliseconds;
                        }
                        else
                            num2 = sw.ElapsedMilliseconds;
                    }
                    else if (command.Contains("brands"))
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        ProcessSearchBrands(value);
                        sw.Stop();
                        if(!indexCheck)
                        {
                            num1 = sw.ElapsedMilliseconds;
                        }
                        else
                            num2 = sw.ElapsedMilliseconds;
                    }
                    else if (command.Contains("mods"))
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        ProcessSearchMods(value);
                        sw.Stop();
                        if(!indexCheck)
                        {
                            num1 = sw.ElapsedMilliseconds;
                        }
                        else
                            num2 = sw.ElapsedMilliseconds;
                    }
                    else
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        ProcessSearchItems(value);
                        ProcessSearchBrands(value);
                        ProcessSearchCategories(value);
                        ProcessSearchMods(value);
                        sw.Stop();
                        WriteLine($"Elapsed time for all search is: {sw.Elapsed}");
                        if(!indexCheck)
                        {
                            num1 = sw.ElapsedMilliseconds;
                        }
                        else
                            num2 = sw.ElapsedMilliseconds;
                    }
                    if(!indexCheck)
                    {
                        mods.AddingIndexes();
                        items.AddingIndexes();
                        ctgs.AddingIndexes();
                        brands.AddingIndexes();
                        indexCheck = true;
                        WriteLine("Adding indexes");
                    }
                }
                else if(command.Contains("generate plot"))
                {
                    ImageGenerator generator = new ImageGenerator("./data/plot.jpg", items);
                    if(num1 == 0 || num2 == 0)
                    {
                        WriteLine("Search wasn`t done to generate a plot");
                    }
                    else
                        generator.GenerateGraphic(num1, num2);
                }
                else if(command.Contains("predict"))
                {
                    string numVal = GetId(command, 8);
                    int id;
                    if (int.TryParse(numVal, out id))
                    {
                        try
                        {
                            Item item = items.GetById(id);
                            WriteLine(item.ToString());
                            ItemForPrediction newItem = prediction.ItemToItemForPrediction(item, ctgs, brands);
                            generation.GenerateCSVItems(100000, newItem.category, newItem.brand, newItem.cost);
                            MLContext mlContext = new MLContext(seed: 0);

                            var model = prediction.Train(mlContext, @"C:\Database_Coursework\generation\items_prediction.csv");

                            prediction.Evaluate(mlContext, model, @"C:\Database_Coursework\generation\items_prediction.csv");
                            prediction.TestSinglePrediction(mlContext, model, newItem);
                        }
                        catch
                        {
                            WriteLine("Item id isn't correct");
                        }
                    }
                }
                else if(command.Contains("generate docx"))
                {
                    ImageGenerator generator = new ImageGenerator("./data/image.png", items);
                    generator.GenerateImage();
                    GatherInfo info = new GatherInfo(items);
                    info.Main();
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
        public Brand SetBrand(Brand brand)
        {
            Write("Enter a name of updating brand: ");
            string name = ReadLine();
            if (name != "")
            {
                brand.brand = name;
            }
            else
                WriteLine("Name of brand is empty, using previous");
            return brand;
        }
        public Category SetCategory(Category category)
        {
            Write("Enter a name of updating category: ");
            string name = ReadLine();
            if (name != "")
            {
                category.category = name;
            }
            else
                WriteLine("Name of category is empty, using previous");
            return category;
        }
        public Item SetItem(Item item)
        {
            while (true)
            {
                Write("Enter a name of updating item: ");
                string name = ReadLine();
                if (name != "")
                {
                    item.name = name;
                }
                else
                    WriteLine("Name of item is empty, using previous");
                while (true)
                {
                    Write("Enter a cost of updating item: ");
                    string costVal = ReadLine();
                    if (costVal == "")
                    {
                        break;
                    }
                    if (int.TryParse(costVal, out int num))
                    {
                        item.cost = int.Parse(costVal);
                        break;
                    }
                    else
                    {
                        WriteLine("Cost is wrong, enter again!");
                    }
                }
                while (true)
                {
                    Write("Enter year of creation for updating item: ");
                    string yearName = ReadLine();
                    if (int.TryParse(yearName, out item.createYear) && int.Parse(yearName) > 1980 && int.Parse(yearName) < 2022)
                    {
                        item.createYear = int.Parse(yearName);
                        break;
                    }
                    else
                    {
                        WriteLine("Wrong year, please enter again!");
                    }
                }
                break;
            }
            return item;
        }
        public Moderator FillMod()
        {
            Moderator mod = new Moderator();
            while (true)
            {
                Write("Enter a unique name of moderator: ");
                mod.name = ReadLine();
                if (mods.GetUniqueNamesCount(mod.name) == 0)
                {
                    break;
                }
                else
                {
                    WriteLine("We already have this name in DB, enter another one!");
                }
            }
            Write("Enter a password for new moderator: ");
            string password = ReadLine();
            return mod;
        }
        public void ProcessLoginCommands()
        {
            while (true)
            {
                Write("Enter a login command: ");
                string command = ReadLine();
                if (command.Contains("registrate"))
                {
                    Moderator mod = FillMod();
                    int id = autentification.GetRegistration(mod, mod.password);
                    WriteLine($"Id of new mod is: {id}");
                    Moderator moderator = mods.GetById(id);
                    WriteLine($"Welcome, new moderator: {moderator.name}");
                    ProcessCommands();
                }
                else if (command.Contains("authentificate"))
                {
                    Write("Enter your name: ");
                    string name = ReadLine();
                    Write("Enter your password: ");
                    string pasw = ReadLine();
                    int checkAut = autentification.GetAutentification(name, pasw);
                    if(checkAut != -1)
                    {
                        Moderator moderator = mods.GetById(checkAut);
                        WriteLine($"Welcome, moderator: {moderator.name}");
                        ProcessCommands();
                    }
                    else
                        WriteLine("We don't have such moderator, try again, please!");
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
        public void ProcessSearchMods(string value)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Write("Serch Moderators: ");
            List<Moderator> searchMods = mods.GetAllSearch(value);
            Moderator[] modArr = new Moderator[searchMods.Count];
            searchMods.CopyTo(modArr);
            if (modArr.Length == 0)
            {
                Write("no mods in this request");
            }
            WriteLine();
            for (int i = 0; i < modArr.Length; i++)
            {
                WriteLine(modArr[i].ToString());
            }
            sw.Stop();
            WriteLine($"Elapsed time for moderator's search is: {sw.Elapsed}");
        }
        public void ProcessSearchItems(string value)
        {
            int[] measures1;
            while (true)
            {
                Write("Enter measures for cost for item: ");
                string measure1 = ReadLine();
                measures1 = GetMeasures(measure1);
                if (measures1[0] != measures1[1])
                {
                    break;
                }
                else
                {
                    WriteLine("Wrong measures, please enter again!");
                }
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Write("Serch Items: ");
            List<Item> searchIts = items.GetAllSearch(value, measures1);
            Item[] itArr = new Item[searchIts.Count];
            searchIts.CopyTo(itArr);
            if (itArr.Length == 0)
            {
                Write("no items in this request");
            }
            WriteLine();
            for (int i = 0; i < itArr.Length; i++)
            {
                WriteLine(itArr[i].ToString());
            }
            sw.Stop();
            WriteLine($"Elapsed time for item search is: {sw.Elapsed}");
        }
        public void ProcessSearchCategories(string value)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Write("Serch Categories: ");
            List<Category> searchCtgs = ctgs.GetAllSearch(value);
            Category[] ctgArr = new Category[searchCtgs.Count];
            searchCtgs.CopyTo(ctgArr);
            if (ctgArr.Length == 0)
            {
                Write("no categories in this request");
            }
            WriteLine();
            for (int i = 0; i < ctgArr.Length; i++)
            {
                WriteLine(ctgArr[i].ToString());
            }
            sw.Stop();
            WriteLine($"Elapsed time for category search is: {sw.Elapsed}");
        }
        public void ProcessSearchBrands(string value)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Write("Serch Brands: ");
            List<Brand> searchBrands = brands.GetAllSearch(value);
            Brand[] brandArr = new Brand[searchBrands.Count];
            searchBrands.CopyTo(brandArr);
            if (brandArr.Length == 0)
            {
                Write("no brands in this request");
            }
            WriteLine();
            for (int i = 0; i < brandArr.Length; i++)
            {
                WriteLine(brandArr[i].ToString());
            }
            sw.Stop();
            WriteLine($"Elapsed time for brand search is: {sw.Elapsed}");
        }
        public void ProcessGetItem(int id)
        {
            try
            {
                Item item = items.GetById(id);
                WriteLine(item.ToString());
            }
            catch
            {
                WriteLine("Item id isn't correct");
            }
        }
        public void ProcessGetCategory(int id)
        {
            try
            {
                Category category = ctgs.GetById(id);
                WriteLine(category.ToString());
            }
            catch
            {
                WriteLine("Category id isn`t correct");
            }
        }
        public void ProcessGetBrand(int id)
        {
            try
            {
                Brand brand = brands.GetById(id);
                WriteLine(brand.ToString());
            }
            catch
            {
                WriteLine("Brand id isn`t correct");
            }
        }
        public void ProcessGetMod(int id)
        {
            try
            {
                Moderator mod = mods.GetById(id);
                WriteLine(mod.ToString());
            }
            catch
            {
                WriteLine("user id isn`t correct");
            }
        }
        public void ProcessDelItem(int id)
        {
            WriteLine($"Item was deleted? - {items.DeleteById(id)}");
        }
        public void ProcessDelCategory(int id)
        {
            WriteLine($"Category was deleted? - {ctgs.DeleteById(id)}");
            items.DeleteAllByCtgId(id);
        }
        public void ProcessDelBrand(int id)
        {
            WriteLine($"Brand was deleted? - {brands.DeleteById(id)}");
            items.DeleteAllByBrandId(id);
        }
        public void ProcessDelMod(int id)
        {
            WriteLine($"User was deleted? - {mods.DeleteById(id)}");
        }
    }
}
