namespace Database_Coursework
{
    public class Brend
    {
        public string brend;
        public int brend_id;
        public Brend(string brend)
        {
            this.brend = brend;
        }
        public Brend()
        {
            this.brend = "newBR";
        }
        public override string ToString()
        {
            return $"Id: {this.brend_id}, Name of brend: {this.brend}";
        }
    }
}
