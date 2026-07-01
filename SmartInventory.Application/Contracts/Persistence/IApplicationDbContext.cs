using Microsoft.EntityFrameworkCore;
using SmartInventory.Domain.Entities;

namespace SmartInventory.Application.Contracts.Persistence;

public interface IApplicationDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Product> Products { get; }
    DbSet<StockMovement> StockMovements { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<PurchaseOrder> PurchaseOrders { get; }
    DbSet<PurchaseOrderItem> PurchaseOrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
