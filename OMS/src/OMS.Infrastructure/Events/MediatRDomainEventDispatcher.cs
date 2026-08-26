using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using OMS.Application.Common;
using OMS.Application.Common.Interfaces;
using OMS.Domain.Common;
using OMS.Domain.Common.Interfaces;

namespace OMS.Infrastructure.Events
{
    public class MediatRDomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IPublisher _publisher;

        public MediatRDomainEventDispatcher(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task DispatchEventsAsync(
            IEnumerable<Entity> entities,
            CancellationToken cancellationToken = default
        )
        {
            var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();
            foreach (var entity in entities)
            {
                entity.ClearDomainEvents();
            }
            foreach (var domainEvent in domainEvents)
            {
                var notificationType = typeof(DomainEventNotification<>).MakeGenericType(
                    domainEvent.GetType()
                );
                var notification = Activator.CreateInstance(notificationType, domainEvent)!;
                await _publisher.Publish(notification, cancellationToken);
            }
        }
    }
}