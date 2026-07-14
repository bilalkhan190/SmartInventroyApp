using Microsoft.EntityFrameworkCore;
using SmartInventory.Application.Contracts;
using SmartInventory.Application.Contracts.Persistence;
using SmartInventory.Domain.Entities;
using SmartInventory.Infrastructure.Persistance.Configurations;

namespace SmartInventory.Infrastructure.Persistance.Context;

public sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IDomainEventDispatcher? _domainEventDispatcher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IDomainEventDispatcher domainEventDispatcher)
        : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
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
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1) Collect events BEFORE save (entities still tracked)
        var domainEvents = ChangeTracker
            .Entries<BaseEntity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var events = entity.DomainEvents.ToList();
                entity.ClearDomainEvents();
                return events;
            })
            .ToList();

        // 2) Persist aggregate state
        var result = await base.SaveChangesAsync(cancellationToken);

        // 3) Publish events AFTER successful save (side effects see committed data)
        if (_domainEventDispatcher is not null && domainEvents.Count > 0)
        {
            await _domainEventDispatcher.DispatchAsync(domainEvents, cancellationToken);
        }

        return result;
    }
}
