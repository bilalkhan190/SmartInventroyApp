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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
