using System;
using System.Collections.Generic;
using System.Text;

namespace OMS.Application.Common.Interfaces.Services
{
    public interface IInventoryService
    {
        Task DeductInventoryAsync(
            Guid productId,
            int quantity,
            CancellationToken cancellationToken = default
        );
    }
}