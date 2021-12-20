using System;
using System.Collections.Generic;
using Microsoft.ML;
using System.Data.SqlClient;
using RepoCode;
using PredictionLib;
namespace consoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = "Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc";
            courseWorkdbContext context = new courseWorkdbContext();
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
