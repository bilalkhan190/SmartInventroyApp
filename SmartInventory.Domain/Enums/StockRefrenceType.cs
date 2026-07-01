using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartInventory.Domain.Enums
{
    public enum StockReferenceType
    {
        PurchaseOrder,
        GoodsReceipt,
        SalesOrder,
        SalesInvoice,
        CustomerReturn,
        SupplierReturn,
        StockAdjustment,
        StockTransfer,
        OpeningStock
    }
}
