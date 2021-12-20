using Microsoft.ML.Data;
namespace PredictionLib
{
    public class ItemCostPrediction
    {
        [ColumnName("Score")]
        public float cost;
    }
}
