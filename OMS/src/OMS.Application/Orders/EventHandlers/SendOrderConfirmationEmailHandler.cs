using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using OMS.Application.Common;
using OMS.Application.Common.Interfaces.Services;
using OMS.Domain.Common.Interfaces;
using OMS.Domain.Events;

namespace OMS.Application.Orders.EventHandlers
{
    public sealed class SendOrderConfirmationEmailHandler
        : INotificationHandler<DomainEventNotification<OrderPlacedEvent>>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<SendOrderConfirmationEmailHandler> _logger;

        public SendOrderConfirmationEmailHandler(
            IEmailService emailService,
            ILogger<SendOrderConfirmationEmailHandler> logger
        )
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Handle(
            DomainEventNotification<OrderPlacedEvent> notification,
            CancellationToken cancellationToken
        )
        {
            var @event = notification.DomainEvent;
            _logger.LogInformation(
                "Sending confirmation email for order {OrderId} to {Email}",
                @event.OrderId,
                @event.CustomerEmail
            );
            await _emailService.SendOrderConfirmationAsync(
                to: @event.CustomerEmail,
                orderId: @event.OrderId,
                totalAmount: @event.TotalAmount,
                items: @event.Items,
                cancellationToken: cancellationToken
            );
        }
    }
}