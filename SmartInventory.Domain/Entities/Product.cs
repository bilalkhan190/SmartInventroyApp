using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = default!;
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }

        public Inventory? ProductInventory { get; set; }
        public ICollection<StockMovement> StockMovements { get; set; } = [];
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = [];
    }
}
