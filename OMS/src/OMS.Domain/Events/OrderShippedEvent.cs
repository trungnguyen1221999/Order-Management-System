using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Common.Interfaces;
using OMS.Domain.ValueObjects;

namespace OMS.Domain.Events
{
    public sealed record OrderShippedEvent : IDomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;

        // Data the handlers need
        public Guid OrderId { get; init; }
        public Guid CustomerId { get; init; }
        public Email CustomerEmail { get; init; } = null!;
        public string TrackingNumber { get; init; } = string.Empty;
        public DateTime EstimatedDelivery { get; init; }
    }
}