using System;
using System.Collections.Generic;
using System.Text;

namespace OMS.Domain.Common.Interfaces
{
    public interface IDomainEvent
    {
        Guid Id { get; }

        DateTime OccurredOn { get; }
    }
}