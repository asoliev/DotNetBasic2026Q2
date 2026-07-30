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
            Price = 1500.55m,
        };

        int productId = repository.Create(product);

        Product? loaded = repository.GetById(productId);

        Assert.NotNull(loaded);
        Assert.Equal("Laptop", loaded!.Name);
        Assert.Equal(1500.55m, loaded.Price);
    }

    [Fact]
    public void Update_ShouldChangePersistedProduct()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository repository = new(connectionString);

        int productId = repository.Create(new() { Name = "Mouse", Price = 10m });

        bool updated = repository.Update(new()
        {
            Id = productId,
            Name = "Gaming Mouse",
            Price = 99.90m,
        });

        Product? loaded = repository.GetById(productId);

        Assert.True(updated);
        Assert.NotNull(loaded);
        Assert.Equal("Gaming Mouse", loaded!.Name);
        Assert.Equal(99.90m, loaded.Price);
    }

    [Fact]
    public void Delete_ShouldRemoveProduct()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository repository = new(connectionString);

        int productId = repository.Create(new() { Name = "Keyboard", Price = 35m });

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

        repository.Create(new() { Name = "Phone", Price = 500m });
        repository.Create(new() { Name = "Tablet", Price = 700m });

        IReadOnlyCollection<Product> products = repository.GetAll();

        Assert.Equal(2, products.Count);
        Assert.Equal(["Phone", "Tablet"], products.Select(p => p.Name).OrderBy(n => n));
    }
}
