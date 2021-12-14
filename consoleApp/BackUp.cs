using Npgsql;
namespace consoleApp
{
    public class BackUp
    {
        private courseWorkdbContext context;
        private string location;
        public BackUp(string connString)
        {
            this.context = new courseWorkdbContext();
        }
        public void CreateBackupFile(string location)
        {
            
        }
        public void RestoreDatabase()
        {
            context.Database.EnsureCreated();
        }
    }
}
