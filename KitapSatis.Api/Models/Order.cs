namespace KitapSatis.Api.Models
{
    public enum OrderStatus
    {
        Pending = 0,
        Paid = 1,
        Cancelled = 2
    }
    public class Order
    {
        public int Id { get; set; }

        public int UserId { get; set; }              
        public decimal TotalPrice { get; set; }      
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public int BookId { get; set; }          
        public decimal UnitPrice { get; set; }   
        public int Quantity { get; set; } = 1;   

    }
}
