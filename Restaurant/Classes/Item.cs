using SQLite;

namespace Restaurant.Classes
{
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int Price { get; set; }
        public string Category { get; set; }

        public Item() { }

        public Item(string name, string description, int price)
        {
            Name = name;
            Description = description;
            Price = price;
        }

        public Item(string name, string description, int price, string base64)
        {
            Name = name;
            Description = description;
            Price = price;
            Image = base64;
        }
    }
}