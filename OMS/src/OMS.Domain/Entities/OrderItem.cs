using OMS.Domain.Common;
using OMS.Domain.ValueObjects;

namespace OMS.Domain.Entities
{
    public class OrderItem : Entity, IAuditableEntity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } = null!;
        public Money UnitPrice { get; private set; } = null!;
        public int Quantity { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Money Subtotal => UnitPrice.Multiply(Quantity);

        private OrderItem()
        { }

        internal static OrderItem Create(
            Guid orderId,
            Guid productId,
            string productName,
            Money unitPrice,
            int quantity
        )
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than 0");
            if (unitPrice.Amount <= 0)
                throw new DomainException("Price must be greater than 0");

            return new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = productId,
                ProductName = productName,
                UnitPrice = unitPrice,
                Quantity = quantity,
            };
        }

        internal void IncreaseQuantity(int addQuantity)
        {
            if (addQuantity < 0)
                throw new DomainException("Can not Add negative quantity");
            Quantity += addQuantity;
        }

        internal void UpdateQuantity(int newQuantity)
        {
            if (newQuantity <= 0)
                throw new DomainException("Quantity must be greater than 0");
            Quantity = newQuantity;
        }
    }
}