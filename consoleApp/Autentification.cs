using RepoCode;
namespace consoleApp
{
    public class Autentification
    {
        public ModRepository moderators;
        public Autentification(ModRepository moderators)
        {
            this.moderators = moderators;
        }
        public int GetRegistration(Moderator mod, string password)
        {
            long count = moderators.GetUniqueNamesCount(mod.name);
            if(count != 0)
            {
                return -1;
            }
            else
            {
                mod.password = password;
                int mod_id = (int)moderators.Insert(mod);
                return mod_id;
            }
        }
        public int GetAutentification(string name, string password)
        {
            long count = moderators.GetUniqueNamesCount(name);
            if(count == 1)
            {
                Moderator moderator = moderators.GetByName(name);
                bool check = VerifyHash(password, moderator.password);
                if(check)
                {
                    return moderator.mod_id;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                return -1;
            }
        }
        private bool VerifyHash(string input, string output)
        {
            return input == output;
        }
    }
}