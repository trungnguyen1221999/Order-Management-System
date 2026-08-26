using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using OMS.Domain.Entities;
using OMS.Domain.ValueObjects;

namespace OMS.Application.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommand : IRequest<Guid>
    {
        public Guid CustomerId { get; set; }
        public Address ShippingAddress { get; set; } = null!;
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}