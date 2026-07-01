using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory.Domain.Entities
{
    public class SalesOrder : BaseEntity
    {
        public string OrderNo { get; set; } 
        public Guid SupplierId { get; set; }
        public ICollection<Supplier> Suppliers{ get; set; }
    }
}
