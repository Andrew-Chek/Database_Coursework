namespace consoleApp
{
    public class Brand
    {
        public string brand;
        public int brand_id;
        public Brand(string brend)
        {
            this.brand = brend;
        }
        public Brand()
        {
            this.brand = "newBR";
        }
        public override string ToString()
        {
            return $"Id: {this.brand_id}, Name of brend: {this.brand}";
        }
    }
}
