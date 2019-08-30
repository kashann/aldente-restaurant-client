namespace Restaurant.Classes
{
    public class JsonOrderItem
    {
        public string name { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }
        public string observation { get; set; }

        public JsonOrderItem() { }

        public JsonOrderItem(OrderItem item)
        {
            name = item.Name;
            price = item.Price;
            quantity = item.Quantity;
            observation = item.Observation;
        }
    }
}