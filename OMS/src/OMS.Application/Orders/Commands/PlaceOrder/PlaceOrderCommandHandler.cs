using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using OMS.Domain.Entities;
using OMS.Domain.Repositories;

namespace OMS.Application.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PlaceOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(
            PlaceOrderCommand request,
            CancellationToken cancellationToken
        )
        {
            var order = Order.Create(request.CustomerId, request.ShippingAddress, request.Items);
            _orderRepository.Add(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return order.Id;
        }
    }
}