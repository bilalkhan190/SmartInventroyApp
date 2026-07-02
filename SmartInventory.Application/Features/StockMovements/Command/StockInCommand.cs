using SmartInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory.Application.Features.StockMovements.Command
{
    public sealed record StockInCommand(Guid productId ,
        Guid referenceId ,
        StockReferenceType stockReferenceType,
        int quantity ,
        string? reason
        );
   
}
