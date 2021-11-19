using System;
using Npgsql;
using System.Collections.Generic;
namespace Database_Coursework
{
    public class Category_Brend_Repo
    {
        private NpgsqlConnection connection;
        public Category_Brend_Repo(string connString)
        {
            this.connection = new NpgsqlConnection(connString);
        }
        public Category GetCTById(int id)
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
        public Brend GetBRById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM brends WHERE category_id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Brend brend = new Brend();
                brend.brend_id = reader.GetInt32(0);
                brend.brend = reader.GetString(1);
                connection.Close();
                return brend;
            }
            else 
            {
                connection.Close();
                throw new Exception("there are no brends with such id");
            }
        }
        public int DeleteCTById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM categories WHERE category_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public int DeleteBRById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM brends WHERE category_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public object InsertCR(Category category)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO categories (category_id, category) 
                VALUES (@ct_id, @ct_name) RETURNING category_id;
            ";
            command.Parameters.AddWithValue("ct_id", category.category_id);
            command.Parameters.AddWithValue("ct_name", category.category);
            object newId = (object)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public object InsertBR(Brend brend)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO brends (brend_id, brend) 
                VALUES (@br_id, @br_name) RETURNING brend_id;
            ";
            command.Parameters.AddWithValue("br_id", brend.brend_id);
            command.Parameters.AddWithValue("br_name", brend.brend);
            object newId = (object)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public bool UpdateCT(Category category)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE categories SET category = @ct_name WHERE category_id = @ct_id;";
            command.Parameters.AddWithValue("ct_id", category.category_id);
            command.Parameters.AddWithValue("ct_name", category.category);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
        public bool UpdateBR(Brend brend)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE brends SET brend = @br_name WHERE brend_id = @br_id;";
            command.Parameters.AddWithValue("br_id", brend.brend_id);
            command.Parameters.AddWithValue("br_name", brend.brend);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
        public List<Category> GetAllCTSSearch(string value)
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
        public List<Brend> GetAllBRSSearch(string value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM brends WHERE brend LIKE '%' || @value || '%'";
            command.Parameters.AddWithValue("value", value);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Brend> list = new List<Brend>();
            while(reader.Read())
            {
                Brend brend = new Brend();
                brend.brend_id = reader.GetInt32(0);
                brend.brend = reader.GetString(1);
                list.Add(brend);
            }
            connection.Close();
            return list;
        }
    }
}
