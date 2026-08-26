using System.Runtime.CompilerServices;
using OMS.Domain.Common;
using OMS.Domain.Common.Interfaces;
using OMS.Domain.Events;
using OMS.Domain.ValueObjects;

namespace OMS.Domain.Entities
{
    public class Order : Entity, IAuditableEntity
    {
        private List<OrderItem> _items = new();

        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
        public Guid CustomerId { get; private set; }

        public Email CustomerEmail { get; private set; } = null!;
        public Address ShippingAddress { get; private set; } = null!;

        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public OrderStatus Status { get; private set; }

        public Money TotalAmount =>
            new Money(
                _items.Sum(i => i.Subtotal.Amount),
                _items.FirstOrDefault()?.UnitPrice.Currency ?? "EUR"
            );

        private Order()
        { }

        public static Order Create(Guid customerId, Address shippingAddress)
        {
            ArgumentException.ThrowIfNullOrEmpty(customerId.ToString());
            if (shippingAddress is null)
                throw new DomainException("Shipping address cannot be null");

            return new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ShippingAddress = shippingAddress,
                Status = OrderStatus.Draft,
                CreatedAt = DateTime.UtcNow,
            };
        }

        public static Order Create(Guid customerId, Address shippingAddress, List<OrderItem> items)
        {
            ArgumentException.ThrowIfNullOrEmpty(customerId.ToString());
            if (shippingAddress is null)
                throw new DomainException("Shipping address cannot be null");
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                ShippingAddress = shippingAddress,
                Status = OrderStatus.Draft,
                CreatedAt = DateTime.UtcNow,
            };
            foreach (var item in items)
            {
                order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
            }
            var orderPlacedEvent = new OrderPlacedEvent
            {
                OrderId = order.Id,
                CustomerId = customerId,
                CustomerEmail = order.CustomerEmail,
                TotalAmount = order.TotalAmount,
                Items = order
                    ._items.Select(i => new OrderItemSnapshot(
                        i.ProductId,
                        i.ProductName,
                        i.Quantity,
                        i.UnitPrice
                    ))
                    .ToList(),
            };
            order.RaiseDomainEvent(orderPlacedEvent);
            return order;
        }

        public void Ship(string trackingNumber, DateTime estimatedDelivery)
        {
            if (Status != OrderStatus.Confirmed)
                throw new DomainException("Only a confirmed order can be shipped");
            Status = OrderStatus.Shipped;
            var orderShippedEvent = new OrderShippedEvent
            {
                OrderId = Id,
                CustomerEmail = CustomerEmail,
                CustomerId = CustomerId,
                EstimatedDelivery = estimatedDelivery,
                TrackingNumber = trackingNumber,
            };
            RaiseDomainEvent(orderShippedEvent);
        }

        // Business method
        public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
        {
            EnsureOrderIsModifiable();

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
        }

        public void RemoveItem(Guid orderItemId)
        {
            EnsureOrderIsModifiable();
            if (Status != OrderStatus.Draft)
                throw new DomainException("Can only RemoveItem with OrderStatus is Draft");
            ArgumentException.ThrowIfNullOrEmpty(orderItemId.ToString());
            var existingItem = _items.FirstOrDefault(i => i.Id == orderItemId);
            if (existingItem == null)
            {
                throw new DomainException($"OrderItem {orderItemId} not found .");
            }
            _items.Remove(existingItem);
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

        private void EnsureOrderIsModifiable()
        {
            if (Status is OrderStatus.Shipped or OrderStatus.Cancelled)
                throw new DomainException($"Can not modify Order with status {Status}");
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