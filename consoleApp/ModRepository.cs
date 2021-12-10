using System;
using System.Collections.Generic;
using Npgsql;

namespace consoleApp
{
    public class ModRepository
    {
        private NpgsqlConnection connection;
        public ModRepository(string connString)
        {
            this.connection = new NpgsqlConnection(connString);
        }
        public Moderator GetById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM moderators WHERE mod_id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Moderator moderator = new Moderator();
                moderator.mod_id = reader.GetInt32(0);
                moderator.name = reader.GetString(1);
                moderator.password = reader.GetString(2);
                connection.Close();
                return moderator;
            }
            else 
            {
                connection.Close();
                throw new Exception("there is no moderator with such id");
            }
        }
        public int DeleteById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM moderators WHERE mod_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public object Insert(Moderator value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO moderators (name, password) 
                VALUES (@name, @password) RETURNING mod_id;
            ";
            command.Parameters.AddWithValue("name", value.name);
            command.Parameters.AddWithValue("password", value.password);
            object newId = command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public bool Update(Moderator value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE moderators SET name = @name, password = @password WHERE mod_id = @mod_id";
            command.Parameters.AddWithValue("name", value.name);
            command.Parameters.AddWithValue("password", value.password);
            command.Parameters.AddWithValue("mod_id", value.mod_id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
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
            while(reader.Read())
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
            while(reader.Read())
            {
                list.Add(reader.GetString(1));
            }
            connection.Close();
            return list;
        }
    }
}
