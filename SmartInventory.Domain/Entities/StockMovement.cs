using SmartInventory.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory.Domain.Entities
{
    public class StockMovement : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;
        public StockMovementType Type { get; set; }

        public Guid ReferenceId {  get; set; }
        public StockReferenceType StockReferenceType { get; set; }
        public string ReferenceNo{ get; set; }  
        public int Quantity { get; set; }
        public string? Reason { get; set; }
    }
}
