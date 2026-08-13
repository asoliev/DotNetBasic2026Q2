using Microsoft.EntityFrameworkCore;
using Module14_ORM.Domain;
using Module14_ORM.EfCore;

namespace Module14_ORM.Tests;

public sealed class EfOrderRepositoryTests
{
    [Fact]
    public async Task CrudWorkflowWorks()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        DbContextOptions<OrmDbContext> options = new DbContextOptionsBuilder<OrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using OrmDbContext dbContext = new(options);
        RecordingOrderStoredProcedureGateway gateway = new();
        EfProductRepository productRepository = new(dbContext);
        EfOrderRepository repository = new(dbContext, gateway);

        int productId = await productRepository.CreateAsync(new Product
        {
            Name = "Lamp",
            Weight = 1,
            Height = 1,
            Width = 1,
            Length = 1,
        }, cancellationToken);

        Order order = new()
        {
            Status = OrderStatus.InProgress,
            CreatedDate = new DateTime(2026, 8, 7, 9, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new DateTime(2026, 8, 7, 9, 15, 0, DateTimeKind.Utc),
            ProductId = productId,
        };

        int id = await repository.CreateAsync(order, cancellationToken);

        Order? loaded = await repository.GetByIdAsync(id, cancellationToken);
        Assert.NotNull(loaded);
        Assert.Equal(OrderStatus.InProgress, loaded!.Status);

        order.Id = id;
        order.Status = OrderStatus.Done;
        int updatedRows = await repository.UpdateAsync(order, cancellationToken);
        Assert.Equal(1, updatedRows);

        loaded = await repository.GetByIdAsync(id, cancellationToken);
        Assert.NotNull(loaded);
        Assert.Equal(OrderStatus.Done, loaded!.Status);

        int deletedRows = await repository.DeleteAsync(id, cancellationToken);
        Assert.Equal(1, deletedRows);
        Assert.Null(await repository.GetByIdAsync(id, cancellationToken));
    }

    [Theory]
    [MemberData(nameof(OrderFilterGatewayTestData.Cases), MemberType = typeof(OrderFilterGatewayTestData))]
    public async Task FilterMethodsDelegateToGateway(OrderFilterGatewayTestCase testCase)
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        DbContextOptions<OrmDbContext> options = new DbContextOptionsBuilder<OrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using OrmDbContext dbContext = new(options);
        RecordingOrderStoredProcedureGateway gateway = new()
        {
            OrdersToReturn = testCase.OrdersToReturn,
            RowsAffectedToReturn = testCase.RowsAffectedToReturn,
        };

        EfOrderRepository repository = new(dbContext, gateway);

        IReadOnlyList<Order> orders = await repository.GetFilteredAsync(testCase.Filter, cancellationToken);
        Assert.Same(testCase.OrdersToReturn, orders);
        Assert.Equal(testCase.Filter, gateway.LastFilter);

        int deletedRows = await repository.DeleteFilteredAsync(testCase.Filter, cancellationToken);
        Assert.Equal(testCase.RowsAffectedToReturn, deletedRows);
        Assert.Equal(testCase.Filter, gateway.LastFilter);
    }
}