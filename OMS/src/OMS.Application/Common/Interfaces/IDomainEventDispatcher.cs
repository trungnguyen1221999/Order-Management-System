using System;
using System.Collections.Generic;
using System.Text;
using OMS.Domain.Common;

namespace OMS.Application.Common.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task DispatchEventsAsync(
            IEnumerable<Entity> entities,
            CancellationToken cancellationToken = default
        );
    }
}