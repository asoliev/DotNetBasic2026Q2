using Microsoft.EntityFrameworkCore;
using Module14_ORM.Domain;
using Module14_ORM.EfCore;

namespace Module14_ORM.Tests;

public sealed class EfOrderRepositoryTests
{
    [Fact]
    public async Task CrudWorkflowWorks()
    {
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
        });

        Order order = new()
        {
            Status = OrderStatus.InProgress,
            CreatedDate = new DateTime(2026, 8, 7, 9, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new DateTime(2026, 8, 7, 9, 15, 0, DateTimeKind.Utc),
            ProductId = productId,
        };

        int id = await repository.CreateAsync(order);

        Order? loaded = await repository.GetByIdAsync(id);
        Assert.NotNull(loaded);
        Assert.Equal(OrderStatus.InProgress, loaded!.Status);

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
        DbContextOptions<OrmDbContext> options = new DbContextOptionsBuilder<OrmDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using OrmDbContext dbContext = new(options);
        RecordingOrderStoredProcedureGateway gateway = new()
        {
            OrdersToReturn =
            [
                new Order { Id = 7, Status = OrderStatus.Done, ProductId = 9 }
            ],
            RowsAffectedToReturn = 2,
        };

        EfOrderRepository repository = new(dbContext, gateway);
        OrderFilter filter = new(Month: 8, Status: OrderStatus.Done, Year: 2026, ProductId: 9);

        IReadOnlyList<Order> orders = await repository.GetFilteredAsync(filter);
        Assert.Single(orders);
        Assert.Equal(7, orders[0].Id);
        Assert.Equal(filter, gateway.LastFilter);

        int deletedRows = await repository.DeleteFilteredAsync(filter);
        Assert.Equal(2, deletedRows);
        Assert.Equal(filter, gateway.LastFilter);
    }
}