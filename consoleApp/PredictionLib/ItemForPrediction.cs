using Microsoft.ML.Data;
namespace PredictionLib
{
    public class ItemForPrediction
    {
        [LoadColumn(0)]
        public int item_id;
        [LoadColumn(1)]
        public string name;
        [LoadColumn(2)]
        public float cost;
        [LoadColumn(3)]
        public string brand;
        [LoadColumn(4)]
        public string category;
        [LoadColumn(5)]
        public int createYear;
        public ItemForPrediction(string name, float cost, string brand, string category, int createYear)
        {
            this.name = name;
            this.cost = cost;
            this.brand = brand;
            this.category = category;
            this.createYear = createYear;
        }
        public ItemForPrediction(int id, string name, float cost, string brand, string category, int createYear)
        {
            this.item_id = id;
            this.name = name;
            this.cost = cost;
            this.brand = brand;
            this.category = category;
            this.createYear = createYear;
        }
        public ItemForPrediction()
        {
            this.name = "Beta item";
            this.cost = 1000;
            this.brand = "Nike";
            this.category = "footwear";
            this.createYear = 2018;
        }
        public override string ToString()
        {
            return $"{this.item_id},{this.name},{this.cost},{this.brand},{this.category},{this.createYear}";
        }
    }
}
