using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace OMS.Application.Orders.EventHandlers
{
    internal sealed class SendOrderConfirmationEmailHandler
        : INotificationHandler<DomainEventNotification>
    { }
}