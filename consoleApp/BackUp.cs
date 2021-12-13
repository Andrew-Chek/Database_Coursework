using Npgsql;
namespace consoleApp
{
    public class BackUp
    {
        private string connString = "Host=localhost;Port=5432;Database=courseWorkdb;Username=postgres;Password=2003Lipovetc";
        private NpgsqlConnection connection;
        private string location;
        public BackUp()
        {
            this.connection = new NpgsqlConnection(connString);
        }
        public void CreateBackupFile(string location)
        {
            
        }
        public void RestoreDatabase()
        {
            
        }
    }
}
