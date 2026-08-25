using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Common.Interfaces;
using OMS.Domain.ValueObjects;

namespace OMS.Domain.Events
{
    public sealed record OrderPlacedEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        // Data the handlers need
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public Money TotalAmount { get; init; } = Money.Zero("EUR");
        public Email CustomerEmail { get; init; } = null!;

        public IReadOnlyList<OrderItemSnapshot> Items { get; init; } = [];
    }

    public sealed record OrderItemSnapshot(
        Guid ProductId,
        string ProductName,
        int Quantity,
        Money UnitPrice
    );
}