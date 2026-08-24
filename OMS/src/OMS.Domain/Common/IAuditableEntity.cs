using System;
using System.Collections.Generic;
using System.Text;

namespace OMS.Domain.Common
{
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; }
        DateTime? UpdatedAt { get; }
    }
}