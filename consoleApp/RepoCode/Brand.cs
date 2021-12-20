namespace RepoCode
{
    public class Brand
    {
        public string brand;
        public int brand_id;
        public virtual ICollection<Item> Items { get; set; }
        public Brand(string brand)
        {
            this.brand = brand;
        }
        public Brand()
        {
            Items = new HashSet<Item>();
        }
        public override string ToString()
        {
            return $"Id: {this.brand_id}, Name of brend: {this.brand}";
        }
    }
}
