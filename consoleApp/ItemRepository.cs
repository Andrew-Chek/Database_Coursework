using System;
using static System.Console;
using Npgsql;
using System.Collections.Generic;

namespace consoleApp
{
    public class ItemRepository
    {
        private NpgsqlConnection connection;
        private courseWorkdbContext context;
        public ItemRepository(string connString, courseWorkdbContext context)
        {
            this.connection = new NpgsqlConnection(connString);
            this.context = context;
        }
        public Item GetById(int id)
        {
            Item item = context.items.Find(id);
            if (item == null)
            {
                throw new NullReferenceException("Cannot find an object with such id.");
            }
            else
                return item;
        }
        public bool DeleteById(int id)
        {
            Item item = context.items.Find(id);
            if (item == null)
            {
                return false;
            }
            else
            {
                context.items.Remove(item);
                context.SaveChanges();
                return true;
            }
        }
        public bool DeleteAllByCtgId(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM items WHERE category_id = @ctg_id";
            command.Parameters.AddWithValue("ctg_id", id);
            int nChanged = (int)command.ExecuteScalar();
            connection.Close();
            return nChanged == 1;
        }
        public bool DeleteAllByBrandId(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM items WHERE brand_id = @brand_id";
            command.Parameters.AddWithValue("brand_id", id);
            int nChanged = (int)command.ExecuteScalar();
            connection.Close();
            return nChanged == 1;
        }
        public object Insert(Item item)
        {
            context.items.Add(item);
            context.SaveChanges();
            return item.item_id;
        }
        public bool Update(Item item)
        {
            try
            {
                Item itemToUpdate = context.items.Find(item.item_id);
                if (itemToUpdate == null)
                    return false;
                else
                    itemToUpdate.name = item.name;
                    itemToUpdate.cost = item.cost;
                    itemToUpdate.category_id = item.category_id;
                    itemToUpdate.brand_id = item.brand_id;
                    itemToUpdate.createYear = item.createYear;
                context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                WriteLine(ex.Message);
                return false;
            }
        }
        public List<Item> GetAllSearch(string value, int[] measures, int year)
        {
            AddingIndexes();
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM items WHERE name LIKE '%' || @value || '%' 
                or cost BETWEEN @a AND @b or createYear = @createYear";
            command.Parameters.AddWithValue("value", value);
            command.Parameters.AddWithValue("a", measures[0]);
            command.Parameters.AddWithValue("b", measures[1]);
            command.Parameters.AddWithValue("createYear", year);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Item> list = new List<Item>();
            while(reader.Read())
            {
                Item item = new Item();
                item.item_id = reader.GetInt32(0);
                item.name = reader.GetString(1);
                item.cost = reader.GetDouble(2);
                item.brand_id = reader.GetInt32(3);
                item.category_id = reader.GetInt32(4);
                item.createYear = reader.GetInt32(5);
                list.Add(item);
            }
            connection.Close();
            return list;
        }
        private void AddingIndexes()
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"
                CREATE INDEX if not exists items_cost_idx ON items (cost);
                CREATE INDEX if not exists items_name_idx ON items using GIN (name);
            ";
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
        }
    }
}
