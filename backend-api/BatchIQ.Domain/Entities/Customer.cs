// Customer entity
namespace BatchIQ.Domain.Entities
{
    public class Customer                    // BatchIQ.Domain/Entities/Customer.cs
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public bool IsInternalStore { get; set; } = false;  // true if Store-01, Store-02

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>(); //
    }
}
