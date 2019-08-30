using System.Collections.Generic;
using System.Linq;
using Android.Util;
using SQLite;

namespace Restaurant.Classes
{
    class Database
    {
        readonly string _folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        public bool CreateDatabase()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, "Menu.db")))
                {
                    connection.CreateTable<Item>();
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public bool InsertIntoTable(Item item)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, "Menu.db")))
                {
                    connection.Insert(item);
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<Item> SelectTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, "Menu.db")))
                {
                    return connection.Table<Item>().ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
        
        public bool RemoveTable()
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, "Menu.db")))
                {
                    connection.DeleteAll<Item>();
                    return true;
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return false;
            }
        }

        public List<Item> SelectTable(string category)
        {
            try
            {
                using (var connection = new SQLiteConnection(System.IO.Path.Combine(_folder, "Menu.db")))
                {
                    return connection.Query<Item>("SELECT * FROM Item Where Category=?", category).ToList();
                }
            }
            catch (SQLiteException ex)
            {
                Log.Info("SQLiteEx", ex.Message);
                return null;
            }
        }
    }
}