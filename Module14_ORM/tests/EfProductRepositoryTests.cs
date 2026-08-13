using Microsoft.EntityFrameworkCore;
using Module14_ORM.Domain;
using Module14_ORM.EfCore;

namespace Module14_ORM.Tests;

public sealed class EfProductRepositoryTests
{
    [Fact]
    public async Task CrudWorkflowWorks()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        DbContextOptions<OrmDbContext> options = new DbContextOptionsBuilder<OrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using OrmDbContext dbContext = new(options);
        EfProductRepository repository = new(dbContext);

        Product product = new()
        {
            Name = "Chair",
            Description = "Office chair",
            Weight = 5,
            Height = 10,
            Width = 15,
            Length = 20,
        };

        int id = await repository.CreateAsync(product, cancellationToken);

        Product? loaded = await repository.GetByIdAsync(id, cancellationToken);
        Assert.NotNull(loaded);
        Assert.Equal("Chair", loaded!.Name);

        product.Id = id;
        product.Name = "Updated Chair";
        int updatedRows = await repository.UpdateAsync(product, cancellationToken);
        Assert.Equal(1, updatedRows);

        loaded = await repository.GetByIdAsync(id, cancellationToken);
        Assert.NotNull(loaded);
        Assert.Equal("Updated Chair", loaded!.Name);

        int deletedRows = await repository.DeleteAsync(id, cancellationToken);
        Assert.Equal(1, deletedRows);
        Assert.Null(await repository.GetByIdAsync(id, cancellationToken));
    }

    [Fact]
    public async Task GetAllAsyncReturnsAllRows()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        DbContextOptions<OrmDbContext> options = new DbContextOptionsBuilder<OrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using OrmDbContext dbContext = new(options);
        EfProductRepository repository = new(dbContext);

        await repository.CreateAsync(new Product { Name = "A", Weight = 1, Height = 1, Width = 1, Length = 1 }, cancellationToken);
        await repository.CreateAsync(new Product { Name = "B", Weight = 2, Height = 2, Width = 2, Length = 2 }, cancellationToken);

        IReadOnlyList<Product> products = await repository.GetAllAsync(cancellationToken);

        Assert.Equal(2, products.Count);
    }
}