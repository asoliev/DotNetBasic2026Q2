using AdoNetLibrary.Models;
using AdoNetLibrary.Repositories;

namespace AdoNetLibrary.Tests;

public sealed class ProductRepositoryTests
{
    [Fact]
    public void CreateAndGetById_ShouldReturnProduct()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository repository = new(connectionString);

        Product product = new()
        {
            Name = "Laptop",
            Description = "Portable workstation",
            Weight = 1.75m,
            Height = 2.00m,
            Width = 32.00m,
            Length = 23.00m,
        };

        int productId = repository.Create(product);

        Product? loaded = repository.GetById(productId);

        Assert.NotNull(loaded);
        Assert.Equal("Laptop", loaded!.Name);
        Assert.Equal("Portable workstation", loaded.Description);
        Assert.Equal(1.75m, loaded.Weight);
        Assert.Equal(2.00m, loaded.Height);
        Assert.Equal(32.00m, loaded.Width);
        Assert.Equal(23.00m, loaded.Length);
    }

    [Fact]
    public void Update_ShouldChangePersistedProduct()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository repository = new(connectionString);

        int productId = repository.Create(new()
        {
            Name = "Mouse",
            Description = "Wireless mouse",
            Weight = 0.10m,
            Height = 3.00m,
            Width = 6.00m,
            Length = 11.00m,
        });

        bool updated = repository.Update(new()
        {
            Id = productId,
            Name = "Gaming Mouse",
            Description = "Gaming wireless mouse",
            Weight = 0.12m,
            Height = 3.20m,
            Width = 6.50m,
            Length = 12.00m,
        });

        Product? loaded = repository.GetById(productId);

        Assert.True(updated);
        Assert.NotNull(loaded);
        Assert.Equal("Gaming Mouse", loaded!.Name);
        Assert.Equal("Gaming wireless mouse", loaded.Description);
        Assert.Equal(0.12m, loaded.Weight);
        Assert.Equal(3.20m, loaded.Height);
        Assert.Equal(6.50m, loaded.Width);
        Assert.Equal(12.00m, loaded.Length);
    }

    [Fact]
    public void Delete_ShouldRemoveProduct()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository repository = new(connectionString);

        int productId = repository.Create(new()
        {
            Name = "Keyboard",
            Description = "Mechanical keyboard",
            Weight = 0.90m,
            Height = 4.00m,
            Width = 14.00m,
            Length = 45.00m,
        });

        bool deleted = repository.Delete(productId);
        Product? loaded = repository.GetById(productId);

        Assert.True(deleted);
        Assert.Null(loaded);
    }

    [Fact]
    public void GetAll_ShouldReturnAllCreatedProducts()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository repository = new(connectionString);

        repository.Create(new()
        {
            Name = "Phone",
            Description = "Smartphone",
            Weight = 0.20m,
            Height = 0.80m,
            Width = 7.50m,
            Length = 15.00m,
        });
        repository.Create(new()
        {
            Name = "Tablet",
            Description = "Tablet device",
            Weight = 0.45m,
            Height = 0.90m,
            Width = 17.00m,
            Length = 25.00m,
        });

        IReadOnlyCollection<Product> products = repository.GetAll();

        Assert.Equal(2, products.Count);
        Assert.Equal(["Phone", "Tablet"], products.Select(p => p.Name).OrderBy(n => n));
    }
}
