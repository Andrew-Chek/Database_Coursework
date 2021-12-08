using System;
using Npgsql;
using System.Collections.Generic;
namespace consoleApp
{
    public class CategoryRepository
    {
        private NpgsqlConnection connection;
        public CategoryRepository(string connString)
        {
            this.connection = new NpgsqlConnection(connString);
        }
        public Category GetById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM categories WHERE category_id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Category category = new Category();
                category.category_id = reader.GetInt32(0);
                category.category = reader.GetString(1);
                connection.Close();
                return category;
            }
            else 
            {
                connection.Close();
                throw new Exception("there are no categories with such id");
            }
        }
        public Category GetByName(string name)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM categories WHERE category = @name";
            command.Parameters.AddWithValue("name", name);
            NpgsqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Category category = new Category();
                category.category_id = reader.GetInt32(0);
                category.category = reader.GetString(1);
                connection.Close();
                return category;
            }
            else 
            {
                connection.Close();
                throw new Exception("there are no categories with such name");
            }
        }
        public int DeleteById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM categories WHERE category_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public object Insert(Category value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO categories (category) 
                VALUES (@ct_name) RETURNING category_id;
            ";
            command.Parameters.AddWithValue("ct_name", value.category);
            object newId = (object)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public bool Update(Category value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE categories SET category = @ct_name WHERE category_id = @ct_id;";
            command.Parameters.AddWithValue("ct_id", value.category_id);
            command.Parameters.AddWithValue("ct_name", value.category);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
        public List<Category> GetAllSearch(string value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM categories WHERE category LIKE '%' || @value || '%'";
            command.Parameters.AddWithValue("value", value);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Category> list = new List<Category>();
            while(reader.Read())
            {
                Category category = new Category();
                category.category_id = reader.GetInt32(0);
                category.category = reader.GetString(1);
                list.Add(category);
            }
            connection.Close();
            return list;
        }
        public long GetUniqueNamesCount(string name)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM categories WHERE category = @name";
            command.Parameters.AddWithValue("name", name);
            long num = (long)command.ExecuteScalar();
            connection.Close();
            return num;
        }
    }
}
