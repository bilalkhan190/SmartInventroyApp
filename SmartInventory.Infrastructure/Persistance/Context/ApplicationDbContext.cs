using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Domain.Entities;
using SmartInventory.Infrastructure.Persistance.Configurations;

namespace SmartInventory.Infrastructure.Persistance.Context;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<DocumentSequences> DocumentSequences => Set<DocumentSequences>();
    public DbSet<GoodReceiveNote> GoodReceiveNotes => Set<GoodReceiveNote>();
    public DbSet<GoodReceiveNoteItem> GoodReceiveNoteItems => Set<GoodReceiveNoteItem>();
    public DbSet<Inventory> Inventories => Set<Inventory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new StockMovementConfiguration());
        modelBuilder.ApplyConfiguration(new SupplierConfiguration());
        modelBuilder.ApplyConfiguration(new PurchaseOrderConfiguration());
        modelBuilder.ApplyConfiguration(new PurchaseOrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentSequenceConfiguration());
        modelBuilder.ApplyConfiguration(new GoodReceiveNoteConfiguration());
        modelBuilder.ApplyConfiguration(new GoodReceiveNoteItemConfiguration());
        modelBuilder.ApplyConfiguration(new InventoryConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
