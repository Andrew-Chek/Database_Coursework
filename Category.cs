namespace Database_Coursework
{
    public class Category
    {
        public string category;
        public int category_id;
        public Category(string category)
        {
            this.category = category;
        }
        public Category()
        {
            this.category = "newCT";
        }
        public override string ToString()
        {
            return $"Id: {this.category_id}, Name of category: {this.category}";
        }
    }
}
