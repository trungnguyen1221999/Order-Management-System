using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using OMS.Domain.Common.Interfaces;

namespace OMS.Application.Common
{
    // Wrapper that bridges IDomainEvent to INotification
    public sealed record DomainEventNotification<TDomainEvent>(TDomainEvent DomainEvent)
        : INotification
        where TDomainEvent : IDomainEvent;
}