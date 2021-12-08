namespace consoleApp
{
    public class Item
    {
        public int item_id;
        public string name;
        public double cost;
        public int brand_id;
        public int category_id;
        public Item(string name, double cost, int brand_id, int category_id)
        {
            this.name = name;
            this.cost = cost;
            this.brand_id = brand_id;
            this.category_id = category_id;
        }
        public Item()
        {
            this.name = "";
            this.cost = 0;
            this.brand_id = 0;
            this.category_id = 0;
        }
        public override string ToString()
        {
            return $"Id: {this.item_id}, Name: {this.name}";
        }
    }
}
