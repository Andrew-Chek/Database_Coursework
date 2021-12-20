using Microsoft.ML;
using RepoCode;
namespace PredictionLib
{
    public class CostPrediction
    {
        public ItemForPrediction ItemToItemForPrediction(Item item, CategoryRepository ctgs, BrandRepository brands)
        {
            Category category = ctgs.GetById(item.category_id);
            Brand brand = brands.GetById(item.brand_id);
            ItemForPrediction predict_item = new ItemForPrediction();
            predict_item.item_id = item.item_id;
            predict_item.name = item.name;
            predict_item.cost = (float)item.cost;
            predict_item.brand = brand.brand;
            predict_item.category = category.category;
            predict_item.createYear = item.createYear;
            return predict_item;
        }
        public ITransformer Train(MLContext mlContext, string dataPath)
        {
            // The IDataView object holds the training dataset 
            IDataView dataView = mlContext.Data.LoadFromTextFile<ItemForPrediction>(dataPath, hasHeader: true, separatorChar: ',');

            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "cost")
            .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "ItemIdEncoded", inputColumnName: "item_id"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "NameEncoded", inputColumnName: "name"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "BrandEncoded", inputColumnName: "brand"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CategoryEncoded", inputColumnName: "category"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CreateYearEncoded", inputColumnName: "createYear"))
                .Append(mlContext.Transforms.Concatenate("Features", "BrandEncoded", "CategoryEncoded", "CreateYearEncoded"))
                .Append(mlContext.Regression.Trainers.FastTree());
                    
            //Create the model
            var model = pipeline.Fit(dataView);

            //Return the trained model
            return model;
        }
        public void Evaluate(MLContext mlContext, ITransformer model, string _testDataPath)
        {
            IDataView dataView = mlContext.Data.LoadFromTextFile<ItemForPrediction>(_testDataPath, hasHeader: true, separatorChar: ',');
            var predictions = model.Transform(dataView);
            var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics output         ");
            Console.WriteLine($"*------------------------------------------------");

            Console.WriteLine($"*       R-Squared Score:      {metrics.RSquared:0.###}");

            Console.WriteLine($"*       Root-Mean-Squared Error:      {metrics.RootMeanSquaredError:#.###}");
            Console.WriteLine("Press Enter to continue...");
            Console.ReadLine();
        }
        public void TestSinglePrediction(MLContext mlContext, ITransformer model, ItemForPrediction item)
        {
            var predictionFunction = mlContext.Model.CreatePredictionEngine<ItemForPrediction, ItemCostPrediction>(model);

            //Create a single TaxiTrip object to be used for predictin
            //Make a prediction
            var prediction = predictionFunction.Predict(item);

            Console.WriteLine($"**********************************************************************");
            Console.WriteLine($"Predicted fare is: {prediction.cost:0.####}, while actual fare: {item.cost}");
            Console.WriteLine($"**********************************************************************");
        }
    }
}
