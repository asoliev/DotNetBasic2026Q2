using Module14_ORM.Dapper;
using Module14_ORM.Domain;

namespace Module14_ORM.Tests;

public sealed class DapperProductRepositoryTests
{
    [Fact]
    public async Task CrudWorkflowWorks()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using SqliteTestDatabase database = await SqliteTestDatabase.CreateAsync();
        DapperProductRepository repository = new(database.ConnectionFactory);

        Product product = new()
        {
            Name = "Box",
            Description = "Cardboard box",
            Weight = 1.25m,
            Height = 2.50m,
            Width = 3.75m,
            Length = 4.00m,
        };

        int id = await repository.CreateAsync(product, cancellationToken);

        Product? loaded = await repository.GetByIdAsync(id, cancellationToken);
        Assert.NotNull(loaded);
        Assert.Equal("Box", loaded!.Name);

        product.Name = "Updated Box";
        product.Description = "Updated description";
        int updatedRows = await repository.UpdateAsync(product, cancellationToken);
        Assert.Equal(1, updatedRows);

        loaded = await repository.GetByIdAsync(id, cancellationToken);
        Assert.NotNull(loaded);
        Assert.Equal("Updated Box", loaded!.Name);

        int deletedRows = await repository.DeleteAsync(id, cancellationToken);
        Assert.Equal(1, deletedRows);
        Assert.Null(await repository.GetByIdAsync(id, cancellationToken));
    }

    [Fact]
    public async Task GetAllAsyncReturnsAllRows()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using SqliteTestDatabase database = await SqliteTestDatabase.CreateAsync();
        DapperProductRepository repository = new(database.ConnectionFactory);

        await repository.CreateAsync(new Product { Name = "A", Weight = 1, Height = 1, Width = 1, Length = 1 }, cancellationToken);
        await repository.CreateAsync(new Product { Name = "B", Weight = 2, Height = 2, Width = 2, Length = 2 }, cancellationToken);

        IReadOnlyList<Product> products = await repository.GetAllAsync(cancellationToken);

        Assert.Equal(2, products.Count);
        Assert.Equal(["A", "B"], products.Select(product => product.Name).ToArray());
    }
}