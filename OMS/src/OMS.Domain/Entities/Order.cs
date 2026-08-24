using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Common;
using OMS.Domain.ValueObjects;

namespace OMS.Domain.Entities
{
    public class Order : Entity, IAuditableEntity
    {
        private List<OrderItem> _items = new();

        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
        public Guid CustomerId { get; private set; }
        public Money TotalAmount { get; private set; } = null!;
        public Address ShippingAddress { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public OrderStatus Status { get; private set; }

        private Order()
        { }

        public Order Create(Guid customerId, Address shippingAddress)
        {
            ArgumentException.ThrowIfNullOrEmpty(customerId.ToString());
            return new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                TotalAmount = Money.Zero("EUR"),
                ShippingAddress = shippingAddress,
                Status = OrderStatus.Draft,
                CreatedAt = DateTime.UtcNow,
            };
        }

        // Business method
        public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
        {
            ArgumentException.ThrowIfNullOrEmpty(productId.ToString());
            ArgumentException.ThrowIfNullOrEmpty(productName);

            if (quantity < 0)
                throw new DomainException("Can not AddItem has negative quantity");
            if (Status != OrderStatus.Draft)
                throw new DomainException("Can only AddItem with OrderStatus is Draft");

            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
            }
            else
            {
                var newOrderItem = OrderItem.Create(
                    Id,
                    productId,
                    productName,
                    unitPrice,
                    quantity
                );
                _items.Add(newOrderItem);
            }
            RecalculateTotal();
        }

        public void RemoveItem(Guid orderItemId)
        {
            ArgumentException.ThrowIfNullOrEmpty(orderItemId.ToString());
            if (Status != OrderStatus.Draft)
                throw new DomainException("Can only RemoveItem with OrderStatus is Draft");
            var existingItem = _items.FirstOrDefault(i => i.Id == orderItemId);
            if (existingItem == null)
            {
                throw new DomainException($"OrderItem {orderItemId} not found .");
            }
            _items.Remove(existingItem);
            RecalculateTotal();
        }

        public void PlaceOrder()
        {
            if (Status != OrderStatus.Draft)
                throw new DomainException("Can only PlaceOrder with OrderStatus is Draft");
            if (!_items.Any())
                throw new DomainException("Can not PlaceOrder without OrderItem");

            Status = OrderStatus.Placed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateShippingAddress(Address newAddress)
        {
            if (Status != OrderStatus.Draft && Status != OrderStatus.Placed)
                throw new DomainException(
                    "Can only UpdateShippingAddress with OrderStatus is Draft or Placed"
                );
            ShippingAddress = newAddress;
        }

        private void RecalculateTotal()
        {
            TotalAmount = _items.Aggregate(
                Money.Zero("EUR"),
                (sum, item) => sum.Add(item.Subtotal)
            );
        }
    }

    public enum OrderStatus
    {
        Draft = 0,
        Placed = 1,
        Confirmed = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
    }
}