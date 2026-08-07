using Module14_ORM.Dapper;
using Module14_ORM.Domain;

namespace Module14_ORM.Tests;

public sealed class DapperOrderRepositoryTests
{
    [Fact]
    public async Task CrudWorkflowWorks()
    {
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
        });

        Order order = new()
        {
            Status = OrderStatus.Loading,
            CreatedDate = new DateTime(2026, 8, 7, 10, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new DateTime(2026, 8, 7, 10, 5, 0, DateTimeKind.Utc),
            ProductId = productId,
        };

        int id = await repository.CreateAsync(order);

        Order? loaded = await repository.GetByIdAsync(id);
        Assert.NotNull(loaded);
        Assert.Equal(OrderStatus.Loading, loaded!.Status);

        order.Id = id;
        order.Status = OrderStatus.Done;
        int updatedRows = await repository.UpdateAsync(order);
        Assert.Equal(1, updatedRows);

        loaded = await repository.GetByIdAsync(id);
        Assert.NotNull(loaded);
        Assert.Equal(OrderStatus.Done, loaded!.Status);

        int deletedRows = await repository.DeleteAsync(id);
        Assert.Equal(1, deletedRows);
        Assert.Null(await repository.GetByIdAsync(id));
    }

    [Fact]
    public async Task FilterMethodsDelegateToGateway()
    {
        await using SqliteTestDatabase database = await SqliteTestDatabase.CreateAsync();
        RecordingOrderStoredProcedureGateway gateway = new()
        {
            OrdersToReturn =
            [
                new Order { Id = 42, Status = OrderStatus.Arrived, ProductId = 7 }
            ],
            RowsAffectedToReturn = 3,
        };

        DapperOrderRepository repository = new(database.ConnectionFactory, gateway);
        OrderFilter filter = new(Month: 8, Status: OrderStatus.Arrived, Year: 2026, ProductId: 7);

        IReadOnlyList<Order> orders = await repository.GetFilteredAsync(filter);
        Assert.Single(orders);
        Assert.Equal(42, orders[0].Id);
        Assert.Equal(filter, gateway.LastFilter);

        int deletedRows = await repository.DeleteFilteredAsync(filter);
        Assert.Equal(3, deletedRows);
        Assert.Equal(filter, gateway.LastFilter);
    }
}