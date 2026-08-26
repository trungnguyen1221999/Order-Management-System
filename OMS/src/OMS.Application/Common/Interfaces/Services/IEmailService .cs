using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Entities;
using OMS.Domain.Events;
using OMS.Domain.ValueObjects;

namespace OMS.Application.Common.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendOrderConfirmationAsync(
            Email to,
            Guid orderId,
            Money totalAmount,
            IReadOnlyList<OrderItemSnapshot> items,
            CancellationToken cancellationToken = default
        );
    }
}