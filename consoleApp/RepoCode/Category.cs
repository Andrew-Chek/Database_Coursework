namespace RepoCode
{
    public class Category
    {
        public string category;
        public int category_id;
        public virtual ICollection<Item> items { get; set; }
        public Category(string category)
        {
            this.category = category;
        }
        public Category()
        {
            items = new HashSet<Item>();
        }
        public override string ToString()
        {
            return $"Id: {this.category_id}, Name of category: {this.category}";
        }
    }
}
