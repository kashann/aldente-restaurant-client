namespace Restaurant.Classes
{
    public class OrderItem : Item
    {
        public int Quantity { get; set; }
        public string Observation { get; set; }

        public OrderItem() { }

        public OrderItem(Item item) : base(item.Name, item.Description, item.Price, item.Image) { }

        public OrderItem(string name, string description, int price, string image, int quantity, string observation) : base(name, description, price, image)
        {
            Quantity = quantity;
            Observation = observation;
        }
    }
}