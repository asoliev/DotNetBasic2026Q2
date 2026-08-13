using Module14_ORM.Dapper;
using Module14_ORM.Domain;

namespace Module14_ORM.Tests;

public sealed class DapperOrderRepositoryTests
{
    [Fact]
    public async Task CrudWorkflowWorks()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;

        await using SqliteTestDatabase database = await SqliteTestDatabase.CreateAsync();
        DapperProductRepository productRepository = new(database.ConnectionFactory);
        RecordingOrderStoredProcedureGateway gateway = new();
        DapperOrderRepository repository = new(database.ConnectionFactory, gateway);

        int productId = await productRepository.CreateAsync(new Product
        {
            Name = "Widget",
            Weight = 1,
            Height = 1,
            Width = 1,
            Length = 1,
        }, cancellationToken);

        Order order = new()
        {
            Status = OrderStatus.Loading,
            CreatedDate = new DateTime(2026, 8, 7, 10, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new DateTime(2026, 8, 7, 10, 5, 0, DateTimeKind.Utc),
            ProductId = productId,
        };

        int id = await repository.CreateAsync(order, cancellationToken);

        Order? loaded = await repository.GetByIdAsync(id, cancellationToken);
        Assert.NotNull(loaded);
        Assert.Equal(OrderStatus.Loading, loaded!.Status);

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

        await using SqliteTestDatabase database = await SqliteTestDatabase.CreateAsync();
        RecordingOrderStoredProcedureGateway gateway = new()
        {
            OrdersToReturn = testCase.OrdersToReturn,
            RowsAffectedToReturn = testCase.RowsAffectedToReturn,
        };

        DapperOrderRepository repository = new(database.ConnectionFactory, gateway);

        IReadOnlyList<Order> orders = await repository.GetFilteredAsync(testCase.Filter, cancellationToken);
        Assert.Same(testCase.OrdersToReturn, orders);
        Assert.Equal(testCase.Filter, gateway.LastFilter);

        int deletedRows = await repository.DeleteFilteredAsync(testCase.Filter, cancellationToken);
        Assert.Equal(testCase.RowsAffectedToReturn, deletedRows);
        Assert.Equal(testCase.Filter, gateway.LastFilter);
    }
}