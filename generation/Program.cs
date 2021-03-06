using System;
using RepoCode;
using PredictionLib;
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
            CostPrediction prediction = new CostPrediction();
            Generation generation = new Generation(connString, items, ctgs, brands, mods, prediction);
            ConsoleLog console = new ConsoleLog(generation);
            console.ProcessCommands();
        }
    }
}
