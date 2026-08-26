using System;
using System.Collections.Generic;
using System.Text;
using OMS.Application.Common.Interfaces;
using OMS.Domain.Common;
using OMS.Domain.Repositories;

namespace OMS.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDomainEventDispatcher _domainEventDispatcher;
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(
            ApplicationDbContext dbContext,
            IDomainEventDispatcher domainEventDispatcher
        )
        {
            _dbContext = dbContext;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Gather every entity with domain events
            var entitiesWithEvents = _dbContext
                .ChangeTracker.Entries<Entity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity)
                .ToList();

            //Save changes to the database
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            // Dispatch domain events
            await _domainEventDispatcher.DispatchEventsAsync(entitiesWithEvents, cancellationToken);

            return result;
        }
    }
}