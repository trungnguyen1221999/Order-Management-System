using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using OMS.Application.Common;
using OMS.Application.Common.Interfaces.Services;
using OMS.Domain.Events;

namespace OMS.Application.Orders.EventHandlers
{
    public class DeductInventoryHandler
        : INotificationHandler<DomainEventNotification<OrderPlacedEvent>>
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<DeductInventoryHandler> _logger;

        public DeductInventoryHandler(
            IInventoryService inventoryService,
            ILogger<DeductInventoryHandler> logger
        )
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        public async Task Handle(
            DomainEventNotification<OrderPlacedEvent> notification,
            CancellationToken cancellationToken
        )
        {
            var @event = notification.DomainEvent;
            foreach (var item in @event.Items)
            {
                _logger.LogInformation(
                    "Deducting {Quantity} of product {ProductId} from inventory for order {OrderId}",
                    item.Quantity,
                    item.ProductId,
                    @event.OrderId
                );
                await _inventoryService.DeductInventoryAsync(
                    productId: item.ProductId,
                    quantity: item.Quantity,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}