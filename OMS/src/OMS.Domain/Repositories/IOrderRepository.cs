using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Entities;

namespace OMS.Domain.Repositories
{
    public interface IOrderRepository
    {
        //Query - finding orders
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Order?> GetByOrderNumberAsync(
            string orderNumber,
            CancellationToken cancellationToken = default
        );

        Task<List<Order?>> GetByCustomerIdAsync(
            Guid customerId,
            CancellationToken cancellationToken = default
        );

        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);

        //Command - changing orders data
        void Add(Order order);

        void Update(Order order);

        void Remove(Order order);
    }
}