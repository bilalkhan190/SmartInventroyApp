using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory.Application.Contracts
{
    public interface IDocumentNumberGenerator
    {
        Task<string> NextPurchaseOrderNoAsync(CancellationToken cancellationToken = default);
        Task<string> NextGrnNoAsync(CancellationToken cancellationToken = default);
    }
}
