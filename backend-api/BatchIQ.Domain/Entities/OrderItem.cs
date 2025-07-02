// OrderItem entity
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities
{
    public class OrderItem                   // BatchIQ.Domain/Entities/OrderItem.cs
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }

        public decimal Quantity { get; set; }
        public UoM UnitOfMeasure { get; set; }
    }
}
