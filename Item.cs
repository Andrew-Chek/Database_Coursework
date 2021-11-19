namespace Database_Coursework
{
    public class Item
    {
        public int item_id;
        public string name;
        public double cost;
        public int brend_id;
        public int category_id;
        public Item(string name, double cost, int brend_id, int category_id)
        {
            this.name = name;
            this.cost = cost;
            this.brend_id = brend_id;
            this.category_id = category_id;
        }
        public Item()
        {
            this.name = "";
            this.cost = 0;
            this.brend_id = 0;
            this.category_id = 0;
        }
        public override string ToString()
        {
            return $"Id: {this.item_id}, Name: {this.name}";
        }
    }
}
