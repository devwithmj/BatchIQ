// Order entity
namespace BatchIQ.Domain.Entities
{
    public class Order                       // BatchIQ.Domain/Entities/Order.cs
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = default!; // Navigation property to Customer
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? OrderDate { get; set; } = null; // Nullable if not yet placed
        public DateTime? ShippedOn { get; set; } = null; // Nullable if not yet shipped
        public DateTime? CompletedOn { get; set; } = null; // Nullable if
        public string Status { get; set; } = "Pending";     // Enum/string as you prefer

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
