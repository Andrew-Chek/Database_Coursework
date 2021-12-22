using RepoCode;
using ScottPlot;
namespace consoleApp
{ 
    public class ImageGenerator
    {
        private string filePath;
        private ItemRepository items;
        public ImageGenerator(string filePath, ItemRepository items)
        {
            this.filePath = filePath;
            this.items = items;
        }
        public void GenerateImage()
        {
            var plt = new ScottPlot.Plot(600, 400);
            double[] values = items.GetItemCosts();
            Item minItem = items.GetMinValue();
            Item maxItem = items.GetMaxValue();
            var hist = new ScottPlot.Statistics.Histogram(values, min: minItem.cost, max: maxItem.cost);

            // plot the bins as a bar graph (on the primary Y axis)
            var bar = plt.AddBar(hist.counts, hist.bins);
            bar.BarWidth = hist.binSize * 1.5; // oversize to reduce render artifacts
            bar.BorderLineWidth = 0;
            bar.YAxisIndex = 0;
            plt.YAxis.Label("Count (#)");
            plt.YAxis.Color(bar.FillColor);

            // plot the mean curve as a scatter plot (on the secondary Y axis)
            var sp = plt.AddScatter(hist.bins, hist.countsFracCurve);
            sp.MarkerSize = 0;
            sp.LineWidth = 2;
            sp.YAxisIndex = 1;
            plt.YAxis2.Label("Fraction");
            plt.YAxis2.Color(sp.Color);
            plt.YAxis2.Ticks(true);

            // decorate the plot
            plt.XAxis2.Label("Cost on items", bold: true);
            plt.XAxis.Label("Value (units)");
            plt.SetAxisLimits(yMin: 0);
            plt.Grid(lineStyle: LineStyle.Dot);

            plt.SaveFig(filePath);
        }
        public void GenerateGraphic(double num1, double num2)
        {
            var plt = new ScottPlot.Plot(600, 400);

            // generate random data to plot
            int groupCount = 1;
            Random rand = new(0);
            double[] values1 = new double[]{num1};
            double[] values2 = new double[]{num2};
            double[] errors1 = new double[]{0};
            double[] errors2 = new double[]{0};

            // group all data together
            string[] groupNames = {"Indexes usage plot"};
            string[] seriesNames = { "Without indexes", "With indexes"};
            double[][] valuesBySeries = { values1, values2};
            double[][] errorsBySeries = { errors1, errors2};

            // add the grouped bar plots and show a legend
            plt.AddBarGroups(groupNames, seriesNames, valuesBySeries, errorsBySeries);
            plt.Legend(location: Alignment.UpperRight);

            // adjust axis limits so there is no padding below the bar graph
            plt.SetAxisLimits(yMin: 0);

            plt.SaveFig(this.filePath);
        }
    }
}