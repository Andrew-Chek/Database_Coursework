using System;
using RepoCode;
using PredictionLib;
using Microsoft.EntityFrameworkCore;
//pg_dump -U postgres -d courseWorkdb -f C:/Database_Coursework/consoleApp/data/postgre.dump
//psql -U postgres -d restored_db -f C:/Database_Coursework/consoleApp/data/postgre.dump
namespace consoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            string connString = "Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc";
            courseWorkdbContext context = new courseWorkdbContext(connString);
            context.CheckPublication();
            var optionsBuilder = new DbContextOptionsBuilder<courseWorkdbContext>();
            courseWorkdbContext replica = new courseWorkdbContext("Host=localhost;Database=new_db;Username=postgres;Password=2003Lipovetc");
            replica.CreateSubscription();
            ItemRepository items = new ItemRepository(connString, context);
            ModRepository mods = new ModRepository(connString, context);
            CostPrediction prediction = new CostPrediction();
            CategoryRepository ctgs = new CategoryRepository(connString, context);
            BrandRepository brands = new BrandRepository(connString, context);
            Autentification autentification = new Autentification(mods);
            ConsoleLog log = new ConsoleLog(items, ctgs, brands, mods, prediction, autentification);
            log.ProcessLoginCommands();
        }
    }
}
