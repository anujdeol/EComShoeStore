namespace ShoopingWebApplication.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } // User who placed the order
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        // Other order-related properties
        public string OrderStatus { get; set; }
        //pending--confirmed--shipped--delivered 
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
