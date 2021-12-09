using System;
using Npgsql;
using System.Collections.Generic;
namespace consoleApp
{
    public class BrandRepository
    {
        private NpgsqlConnection connection;
        public BrandRepository(string connString)
        {
            this.connection = new NpgsqlConnection(connString);
        }
        public Brand GetById(int id)
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
        public Brand GetByName(string name)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM brands WHERE brand = @name";
            command.Parameters.AddWithValue("name", name);
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
                throw new Exception("there are no brands with such name");
            }
        }
        public int DeleteById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM brands WHERE brand_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public object Insert(Brand value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO brands (brand) 
                VALUES (@br_name) RETURNING brand_id;
            ";
            command.Parameters.AddWithValue("br_name", value.brand);
            object newId = (object)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public bool Update(Brand value)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE brands SET brand = @br_name WHERE brand_id = @br_id;";
            command.Parameters.AddWithValue("br_id", value.brand_id);
            command.Parameters.AddWithValue("br_name", value.brand);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
        public long GetUniqueNamesCount(string name)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM brands WHERE brand = @name";
            command.Parameters.AddWithValue("name", name);
            long num = (long)command.ExecuteScalar();
            connection.Close();
            return num;
        }
        public List<Brand> GetAllSearch(string value)
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
        public List<string> GetAllBrands()
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM brands";
            NpgsqlDataReader reader = command.ExecuteReader();
            List<string> list = new List<string>();
            while(reader.Read())
            {
                string brand = reader.GetString(1);
                list.Add(brand);
            }
            connection.Close();
            return list;
        }
    }
}
