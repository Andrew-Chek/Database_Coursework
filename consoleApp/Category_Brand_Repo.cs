using System;
using Npgsql;
using System.Collections.Generic;
namespace consoleApp
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
        public Brand GetBRById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM brands WHERE brand_id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Brand brand = new Brand();
                brand.brand_id = reader.GetInt32(0);
                brand.brand = reader.GetString(1);
                connection.Close();
                return brand;
            }
            else 
            {
                connection.Close();
                throw new Exception("there are no brands with such id");
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
            command.CommandText = "DELETE FROM brands WHERE brand_id = @id";
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
                INSERT INTO categories (category) 
                VALUES (@ct_name) RETURNING category_id;
            ";
            command.Parameters.AddWithValue("ct_name", category.category);
            object newId = (object)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public object InsertBR(Brand brand)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO brands (brand) 
                VALUES (@br_name) RETURNING brand_id;
            ";
            command.Parameters.AddWithValue("br_name", brand.brand);
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
        public bool UpdateBR(Brand brand)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE brands SET brand = @br_name WHERE brand_id = @br_id;";
            command.Parameters.AddWithValue("br_id", brand.brand_id);
            command.Parameters.AddWithValue("br_name", brand.brand);
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
        public List<Brand> GetAllBRSSearch(string value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM brands WHERE brand LIKE '%' || @value || '%'";
            command.Parameters.AddWithValue("value", value);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Brand> list = new List<Brand>();
            while(reader.Read())
            {
                Brand brand = new Brand();
                brand.brand_id = reader.GetInt32(0);
                brand.brand = reader.GetString(1);
                list.Add(brand);
            }
            connection.Close();
            return list;
        }
    }
}
