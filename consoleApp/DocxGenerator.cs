using System;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using RepoCode;
using System.Drawing;
namespace consoleApp
{
    public class GatherInfo
    {
        private ItemRepository items;
        public GatherInfo(ItemRepository items)
        {
            this.items = items;
        }
        public double GetAverage()
        {
            double[] values = items.GetItemCosts();
            double sum = 0;
            for(int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }
            double average = sum / values.Length;
            return average;
        }
        public void Main()
        {
            ClearAndDeleteFolder("./data/output/");
            string zipPath = @"./data/report.docx";
            string extractPath = @"./data/output";
            ZipFile.ExtractToDirectory(zipPath, extractPath);
            XElement root = XElement.Load("./data/output/word/document.xml");
            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                {"{count}", $"{items.GetCount()}"},
                {"{average}", $"{GetAverage()}"},
                {"{name}", $"{items.GetMaxValue().name}"},
                {"{cost}", $"{items.GetMaxValue().cost}"},
                {"{createYear}", $"{items.GetMaxValue().createYear}"},
                {"{name1}", $"{items.GetMinValue().name}"},
                {"{cost1}", $"{items.GetMinValue().cost}"},
                {"{createYear1}", $"{items.GetMinValue().createYear}"}
            };
            Bitmap bmp = new Bitmap("./data/image.png");
            bmp.Save("./data/output/word/media/image1.png");
            FindAndReplace(root, dict);
            root.Save("./data/output/word/document.xml", SaveOptions.DisableFormatting);
            DateTime now = DateTime.Now;
            ZipFile.CreateFromDirectory("./data/output", $"./data/report{now.Minute}.docx");
        }
        public void FindAndReplace(XElement node, Dictionary<string, string> dict)
        {
            if (node.FirstNode != null
                && node.FirstNode.NodeType == XmlNodeType.Text)
            {
                string replacement;
                if (dict.TryGetValue(node.Value, out replacement))
                {
                    node.Value = replacement;
                }
            }
            foreach (XElement el in node.Elements())
            {
                FindAndReplace(el, dict);
            }
        }
        public void ClearAndDeleteFolder(string folderPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            foreach(FileInfo file in dirInfo.GetFiles())
            {
                file.Delete();
            }
            dirInfo.Delete(true);
        }
    }
}