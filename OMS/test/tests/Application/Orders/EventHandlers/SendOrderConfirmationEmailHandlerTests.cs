using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using OMS.Application.Common;
using OMS.Application.Common.Interfaces.Services;
using OMS.Application.Orders.EventHandlers;
using OMS.Domain.Events;
using OMS.Domain.ValueObjects;

namespace OMS.Test.Application.Orders.EventHandlers
{
    public class SendOrderConfirmationEmailHandlerTests
    {
        private readonly IEmailService _emailService = Substitute.For<IEmailService>();

        [Fact]
        public async Task Handle_ShouldSendEmailToCustomer()
        {
            // Arrange
            var handler = new SendOrderConfirmationEmailHandler(
                _emailService,
                NullLogger<SendOrderConfirmationEmailHandler>.Instance
            );

            var @event = new OrderPlacedEvent
            {
                OrderId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                CustomerEmail = Email.Create("abc@gmail.com"),
                TotalAmount = Money.FromEUR(50),
                Items = [],
            };
            var notification = new DomainEventNotification<OrderPlacedEvent>(@event);

            //Act
            await handler.Handle(notification, CancellationToken.None);
            // Assert
            await _emailService
                .Received(1)
                .SendOrderConfirmationAsync(
                    to: @event.CustomerEmail,
                    orderId: @event.OrderId,
                    totalAmount: @event.TotalAmount,
                    items: @event.Items,
                    cancellationToken: CancellationToken.None
                );
        }
    }
}