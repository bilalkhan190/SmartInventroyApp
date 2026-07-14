using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SmartInventory.Domain.Entities;
using SmartInventory.Infrastructure.Identity;
using SmartInventory.Infrastructure.Persistance.Context;

namespace SmartInventory.Infrastructure.Persistance.Seeding;

public sealed class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public DatabaseSeeder(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await SeedCategoriesAsync(cancellationToken);
        await SeedProductsAsync(cancellationToken);
        await SeedSuppliersAsync(cancellationToken);
        await SeedAdminAsync(cancellationToken);
    }

    private async Task SeedAdminAsync(CancellationToken cancellationToken)
    {
        var username = _configuration["SeedAdmin:Username"];
        var password = _configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            return;
        }

        var normalizedUsername = username.Trim().ToUpperInvariant();
        if (await _context.Users.AnyAsync(
            user => user.NormalizedEmail == normalizedUsername && user.DeletedAt == null,
            cancellationToken))
        {
            return;
        }

        _context.Users.Add(new User
        {
            Email = username.Trim(),
            NormalizedEmail = normalizedUsername,
            DisplayName = "Administrator",
            PasswordHash = PasswordHashing.Hash(password)
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        if (await _context.Categories.AnyAsync(x => x.DeletedAt == null ,cancellationToken))
        {
            return;
        }

        var categories = new[]
        {
            new Category { CategoryName = "Electronics" },
            new Category { CategoryName = "Office Supplies" },
            new Category { CategoryName = "Groceries" }
        };

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedProductsAsync(CancellationToken cancellationToken)
    {
        if (await _context.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var categories = await _context.Categories
            .AsNoTracking()
            .Where(category => category.DeletedAt == null)
            .ToDictionaryAsync(category => category.CategoryName, cancellationToken);

        if (categories.Count == 0)
        {
            return;
        }

        if (!categories.ContainsKey("Electronics")
            || !categories.ContainsKey("Office Supplies")
            || !categories.ContainsKey("Groceries"))
        {
            return;
        }

        var products = new[]
        {
            new Product
            {
                Name = "Laptop 15\"",
                Sku = "ELEC-LAP-001",
                Description = "15 inch business laptop",
                CategoryId = categories["Electronics"].Id,
                Quantity = 25,
                ReorderLevel = 5
            },
            new Product
            {
                Name = "Wireless Mouse",
                Sku = "ELEC-MOU-001",
                Description = "Ergonomic wireless mouse",
                CategoryId = categories["Electronics"].Id,
                Quantity = 100,
                ReorderLevel = 20
            },
            new Product
            {
                Name = "A4 Paper Pack",
                Sku = "OFF-PAP-001",
                Description = "500 sheets A4 paper",
                CategoryId = categories["Office Supplies"].Id,
                Quantity = 50,
                ReorderLevel = 10
            },
            new Product
            {
                Name = "Rice 5kg",
                Sku = "GRO-RIC-001",
                Description = "Basmati rice 5kg bag",
                CategoryId = categories["Groceries"].Id,
                Quantity = 200,
                ReorderLevel = 50
            }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync(cancellationToken);

        var inventories = products.Select(product => new Inventory
        {
            ProductId = product.Id,
            CurrentStockQuantity = product.Quantity
        }).ToArray();

        _context.Inventories.AddRange(inventories);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedSuppliersAsync(CancellationToken cancellationToken)
    {
        if (await _context.Suppliers.AnyAsync(cancellationToken))
        {
            return;
        }

        var suppliers = new[]
        {
            new Supplier
            {
                Name = "ABC Wholesale",
                ContactName = "Ahmed Khan",
                Email = "ahmed@abcwholesale.com",
                Phone = "+92-300-1234567",
                Address = "Plot 12, Industrial Area, Karachi"
            },
            new Supplier
            {
                Name = "Global Supplies Ltd",
                ContactName = "Sara Ali",
                Email = "sara@globalsupplies.com",
                Phone = "+92-321-7654321",
                Address = "Warehouse 5, Lahore"
            },
            new Supplier
            {
                Name = "Tech Distributors Inc",
                ContactName = "Bilal Hassan",
                Email = "bilal@techdist.com",
                Phone = "+92-333-9988776",
                Address = "Sector F-7, Islamabad"
            }
        };

        _context.Suppliers.AddRange(suppliers);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
