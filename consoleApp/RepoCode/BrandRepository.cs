using System;
using static System.Console;
using Npgsql;
using System.Collections.Generic;
namespace RepoCode
{
    public class BrandRepository
    {
        private NpgsqlConnection connection;
        private courseWorkdbContext context;
        public BrandRepository(string connString, courseWorkdbContext context)
        {
            this.connection = new NpgsqlConnection(connString);
            this.context = context;
        }
        public Brand GetById(int id)
        {
            Brand brand = context.brands.Find(id);
            if (brand == null)
            {
                throw new NullReferenceException("Cannot find an object with such id.");
            }
            else
                return brand;
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
        public bool DeleteById(int id)
        {
            Brand brand = context.brands.Find(id);
            if (brand == null)
            {
                return false;
            }
            else
            {
                context.brands.Remove(brand);
                Item[] items = FindItemsByBrand(brand);
                for (int i = 0; i < items.Length; i++)
                {
                    context.items.Remove(items[i]);
                }
                context.SaveChanges();
                return true;
            }
        }
        public Item[] FindItemsByBrand(Brand brand)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM items WHERE brand_id = @id";
            command.Parameters.AddWithValue("id", brand.brand_id);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Item> list = new List<Item>();
            while (reader.Read())
            {
                Item item = new Item();
                item.item_id = reader.GetInt32(0);
                item.name = reader.GetString(1);
                item.cost = reader.GetDouble(2);
                item.brand_id = reader.GetInt32(3);
                item.category_id = reader.GetInt32(4);
                list.Add(item);
            }
            connection.Close();
            Item[] array = new Item[list.Count];
            list.CopyTo(array);
            return array;
        }
        public object Insert(Brand value)
        {
            context.brands.Add(value);
            context.SaveChanges();
            return value.brand_id;
        }
        public bool Update(Brand value)
        {
            try
            {
                Brand brandToUpdate = context.brands.Find(value.brand_id);
                if (brandToUpdate == null)
                    return false;
                else
                    brandToUpdate.brand = value.brand;
                context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                WriteLine(ex.Message);
                return false;
            }
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
            AddingIndexes();
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
        public List<string> GetAllNames()
        {
            AddingIndexes();
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM brands";
            NpgsqlDataReader reader = command.ExecuteReader();
            List<string> list = new List<string>();
            while(reader.Read())
            {
                list.Add(reader.GetString(1));
            }
            connection.Close();
            return list;
        }
        private void AddingIndexes()
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"
                CREATE INDEX if not exists brands_name_idx ON brands using GIN (brand);
            ";
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
