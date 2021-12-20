using System;
using static System.Console;
using Npgsql;
using System.Collections.Generic;
namespace RepoCode
{
    public class CategoryRepository
    {
        private NpgsqlConnection connection;
        private courseWorkdbContext context;
        public CategoryRepository(string connString, courseWorkdbContext context)
        {
            this.connection = new NpgsqlConnection(connString);
            this.context = context;
        }
        public Category GetById(int id)
        {
            Category category = context.categories.Find(id);
            if (category == null)
            {
                throw new NullReferenceException("Cannot find an object with such id.");
            }
            else
                return category;
        }
        public Category GetByName(string name)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM categories WHERE category = @name";
            command.Parameters.AddWithValue("name", name);
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
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
        public bool DeleteById(int id)
        {
            Category ctg = context.categories.Find(id);
            if (ctg == null)
            {
                return false;
            }
            else
            {
                context.categories.Remove(ctg);
                Item[] items = FindItemsByCategory(ctg);
                for (int i = 0; i < items.Length; i++)
                {
                    context.items.Remove(items[i]);
                }
                context.SaveChanges();
                return true;
            }
        }
        public Item[] FindItemsByCategory(Category ctg)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM items WHERE category_id = @id";
            command.Parameters.AddWithValue("id", ctg.category_id);
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
        public object Insert(Category value)
        {
            context.categories.Add(value);
            context.SaveChanges();
            return value.category_id;
        }
        public bool Update(Category category)
        {
            try
            {
                Category ctgToUpdate = context.categories.Find(category.category_id);
                if (ctgToUpdate == null)
                    return false;
                else
                    ctgToUpdate.category = category.category;
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message);
                return false;
            }
        }
        public List<Category> GetAllSearch(string value)
        {
            AddingIndexes();
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM categories WHERE category LIKE '%' || @value || '%'";
            command.Parameters.AddWithValue("value", value);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Category> list = new List<Category>();
            while (reader.Read())
            {
                Category category = new Category();
                category.category_id = reader.GetInt32(0);
                category.category = reader.GetString(1);
                list.Add(category);
            }
            connection.Close();
            return list;
        }
        public List<string> GetAllNames()
        {
            AddingIndexes();
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM categories";
            NpgsqlDataReader reader = command.ExecuteReader();
            List<string> list = new List<string>();
            while (reader.Read())
            {
                list.Add(reader.GetString(1));
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
        private void AddingIndexes()
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"
                CREATE INDEX if not exists categories_name_idx ON categories using GIN (category);
            ";
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
