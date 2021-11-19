namespace Database_Coursework
{
    public class Moderator
    {
        public string name;
        public int mod_id;
        public string password;
        public Moderator(string name)
        {
            this.name = name;
        }
        public Moderator(string name, string password)
        {
            this.password = password;
            this.name = name;
        }
        public Moderator()
        {
            this.name = "newMod";
        }
        public override string ToString()
        {
            return $"Id: {this.mod_id}, Name: {this.name}";
        }
    }
}