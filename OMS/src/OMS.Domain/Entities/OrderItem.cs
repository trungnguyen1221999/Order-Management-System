using System;
using System.Collections.Generic;
using System.Text;
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

        internal void IncreaseQuantity(int qnt)
        {
            if (qnt < 0)
                throw new DomainException("Can not Add negative quantity");
            Quantity += qnt;
        }
    }
}