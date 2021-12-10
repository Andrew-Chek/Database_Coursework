namespace consoleApp
{
    public class Item
    {
        public int item_id;
        public string name;
        public double cost;
        public int brand_id;
        public int category_id;
        public virtual Category category { get; set; }
        public virtual Brand brand { get; set; }
        public Item(string name, double cost, int brand_id, int category_id)
        {
            this.name = name;
            this.cost = cost;
            this.brand_id = brand_id;
            this.category_id = category_id;
        }
        public Item()
        {
            this.name = "Beta item";
            this.cost = 1000;
            this.brand_id = 3;
            this.category_id = 3;
        }
        public override string ToString()
        {
            return $"Id: {this.item_id}, Name: {this.name}";
        }
    }
}
