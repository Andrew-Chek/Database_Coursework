using System;
using Npgsql;
using System.Collections.Generic;

namespace Database_Coursework
{
    public class ItemRepository
    {
        private NpgsqlConnection connection;
        public ItemRepository(string connString)
        {
            this.connection = new NpgsqlConnection(connString);
        }
        public Item GetById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM items WHERE item_id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            if(reader.Read())
            {
                Item item = new Item();
                item.item_id = reader.GetInt32(0);
                item.name = reader.GetString(1);
                item.cost = reader.GetDouble(2);
                item.brend_id = reader.GetInt32(3);
                item.category_id = reader.GetInt32(4);
                connection.Close();
                return item;
            }
            else 
            {
                connection.Close();
                throw new Exception("there are no items with such id");
            }
        }
        public int DeleteById(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM items WHERE item_id = @id";
            command.Parameters.AddWithValue("id", id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged;
        }
        public object Insert(Item item)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO items (name, cost, brend_id, category_id) 
                VALUES (@name, @cost, @br_id, @ct_id) RETURNING item_id;
            ";
            command.Parameters.AddWithValue("name", item.name);
            command.Parameters.AddWithValue("cost", item.cost);
            command.Parameters.AddWithValue("br_id", item.brend_id);
            command.Parameters.AddWithValue("ct_id", item.category_id);
            object newId = (object)command.ExecuteScalar();
            connection.Close();
            return newId;
        }
        public bool Update(Item item)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = 
            "Update items SET name = @name, cost = @cost, brend_id = @br_id, category_id = @ct_id WHERE item_id = @id";
            command.Parameters.AddWithValue("cost", item.cost);
            command.Parameters.AddWithValue("name", item.name);
            command.Parameters.AddWithValue("br_id", item.brend_id);
            command.Parameters.AddWithValue("ct_id", item.category_id);
            command.Parameters.AddWithValue("id", item.item_id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;
        }
        public List<Item> GetAllSearch(string value, int[] measures)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM items WHERE name LIKE '%' || @value || '%' 
                or cost BETWEEN @a AND @b";
            command.Parameters.AddWithValue("value", value);
            command.Parameters.AddWithValue("a", measures[0]);
            command.Parameters.AddWithValue("b", measures[1]);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Item> list = new List<Item>();
            while(reader.Read())
            {
                Item item = new Item();
                item.item_id = reader.GetInt32(0);
                item.name = reader.GetString(1);
                item.cost = reader.GetDouble(2);
                item.brend_id = reader.GetInt32(3);
                item.category_id = reader.GetInt32(4);
                list.Add(item);
            }
            connection.Close();
            return list;
        }
    }
}
