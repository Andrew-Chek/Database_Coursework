using System;
using System.Collections.Generic;
using Npgsql;

namespace consoleApp
{
    public class ModRepository
    {
        private NpgsqlConnection connection;
        private courseWorkdbContext context;
        public ModRepository(string connString, courseWorkdbContext context)
        {
            this.connection = new NpgsqlConnection(connString);
            this.context = context;
        }
        public Moderator GetById(int id)
        {
            Moderator moderators = context.moderators.Find(id);
            if (moderators == null)
            {
                throw new NullReferenceException("Cannot find an object with such id.");
            }
            else
                return moderators;
        }
        public bool DeleteById(int id)
        {
            Moderator mod = context.moderators.Find(id);
            if (mod == null)
            {
                return false;
            }
            else
            {
                context.moderators.Remove(mod);
                context.SaveChanges();
                return true;
            }
        }
        public object Insert(Moderator value)
        {
            context.moderators.Add(value);
            context.SaveChanges();
            return value.mod_id;
        }
        public bool Update(Moderator mod)
        {
            Moderator modToUpdate = context.moderators.Find(mod.mod_id);
            if (modToUpdate == null || context.moderators.Find(mod.mod_id) == null)
                return false;
            else if(modToUpdate != null && context.moderators.Find(mod.mod_id) != null)
                modToUpdate.name = mod.name;
                modToUpdate.password = mod.password;
                context.SaveChanges();
                return true;
        }
        public long GetUniqueNamesCount(string name)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM moderators WHERE name = @name";
            command.Parameters.AddWithValue("name", name);
            long num = (long)command.ExecuteScalar();
            connection.Close();
            return num;
        }
        public List<Moderator> GetAllSearch(string value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM moderators WHERE name LIKE '%' || @value || '%' 
                or password LIKE '%' || @value || '%'";
            command.Parameters.AddWithValue("value", value);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Moderator> list = new List<Moderator>();
            while (reader.Read())
            {
                Moderator user = new Moderator();
                user.mod_id = reader.GetInt32(0);
                user.name = reader.GetString(1);
                user.password = reader.GetString(2);
                list.Add(user);
            }
            connection.Close();
            return list;
        }
        public List<string> GetAllNames()
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM moderators";
            NpgsqlDataReader reader = command.ExecuteReader();
            List<string> list = new List<string>();
            while (reader.Read())
            {
                list.Add(reader.GetString(1));
            }
            connection.Close();
            return list;
        }
    }
}
