using AdoNetLibrary.Models;
using AdoNetLibrary.Repositories;

namespace AdoNetLibrary.Tests;

public sealed class OrderRepositoryTests
{
    [Fact]
    public void CreateAndGetById_ShouldReturnOrder()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository productRepository = new(connectionString);
        OrderRepository orderRepository = new(connectionString);

        int productId = productRepository.Create(new() { Name = "Monitor", Price = 280m });

        int orderId = orderRepository.Create(new()
        {
            ProductId = productId,
            Quantity = 3,
            OrderDate = new(2026, 06, 12),
            Status = OrderStatus.InProgress,
        });

        Order? loaded = orderRepository.GetById(orderId);

        Assert.NotNull(loaded);
        Assert.Equal(productId, loaded!.ProductId);
        Assert.Equal(3, loaded.Quantity);
        Assert.Equal(OrderStatus.InProgress, loaded.Status);
        Assert.NotEqual(default, loaded.CreatedDate);
        Assert.NotEqual(default, loaded.UpdatedDate);
        Assert.Equal(loaded.CreatedDate, loaded.UpdatedDate);
    }

    [Fact]
    public void UpdateAndDelete_ShouldModifyThenRemoveOrder()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository productRepository = new(connectionString);
        OrderRepository orderRepository = new(connectionString);

        int productId = productRepository.Create(new() { Name = "SSD", Price = 180m });
        int orderId = orderRepository.Create(new()
        {
            ProductId = productId,
            Quantity = 1,
            OrderDate = new(2026, 05, 01),
            Status = OrderStatus.NotStarted,
        });

        bool updated = orderRepository.Update(new()
        {
            Id = orderId,
            ProductId = productId,
            Quantity = 2,
            OrderDate = new(2026, 05, 02),
            Status = OrderStatus.Loading,
        });

        Order? loadedAfterUpdate = orderRepository.GetById(orderId);
        bool deleted = orderRepository.Delete(orderId);
        Order? loadedAfterDelete = orderRepository.GetById(orderId);

        Assert.True(updated);
        Assert.NotNull(loadedAfterUpdate);
        Assert.Equal(2, loadedAfterUpdate!.Quantity);
        Assert.Equal(OrderStatus.Loading, loadedAfterUpdate.Status);
        Assert.NotEqual(default, loadedAfterUpdate.CreatedDate);
        Assert.NotEqual(default, loadedAfterUpdate.UpdatedDate);
        Assert.True(loadedAfterUpdate.UpdatedDate >= loadedAfterUpdate.CreatedDate);
        Assert.True(deleted);
        Assert.Null(loadedAfterDelete);
    }

    [Fact]
    public void GetOrders_ShouldFilterByMonthYearStatusAndProduct()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository productRepository = new(connectionString);
        OrderRepository orderRepository = new(connectionString);

        int product1 = productRepository.Create(new() { Name = "Product A", Price = 12m });
        int product2 = productRepository.Create(new() { Name = "Product B", Price = 20m });

        orderRepository.Create(new()
        {
            ProductId = product1,
            Quantity = 2,
            OrderDate = new(2026, 04, 10),
            Status = OrderStatus.Arrived,
        });
        orderRepository.Create(new()
        {
            ProductId = product1,
            Quantity = 4,
            OrderDate = new(2026, 04, 20),
            Status = OrderStatus.Done,
        });
        orderRepository.Create(new()
        {
            ProductId = product2,
            Quantity = 5,
            OrderDate = new(2025, 04, 10),
            Status = OrderStatus.Arrived,
        });

        IReadOnlyCollection<Order> filtered = orderRepository.GetOrders(new()
        {
            Month = 4,
            Year = 2026,
            Status = OrderStatus.Arrived,
            ProductId = product1,
        });

        Order order = Assert.Single(filtered);
        Assert.Equal(product1, order.ProductId);
        Assert.Equal(OrderStatus.Arrived, order.Status);
        Assert.Equal(2026, order.OrderDate.Year);
        Assert.Equal(4, order.OrderDate.Month);
        Assert.NotEqual(default, order.CreatedDate);
        Assert.NotEqual(default, order.UpdatedDate);
    }

    [Fact]
    public void DeleteOrders_ShouldBulkDeleteInTransactionByFilter()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository productRepository = new(connectionString);
        OrderRepository orderRepository = new(connectionString);

        int product1 = productRepository.Create(new() { Name = "Product X", Price = 12m });
        int product2 = productRepository.Create(new() { Name = "Product Y", Price = 20m });

        orderRepository.Create(new()
        {
            ProductId = product1,
            Quantity = 2,
            OrderDate = new(2026, 02, 01),
            Status = OrderStatus.Cancelled,
        });
        orderRepository.Create(new()
        {
            ProductId = product1,
            Quantity = 4,
            OrderDate = new(2026, 02, 15),
            Status = OrderStatus.Cancelled,
        });
        orderRepository.Create(new()
        {
            ProductId = product2,
            Quantity = 3,
            OrderDate = new(2026, 02, 20),
            Status = OrderStatus.Cancelled,
        });

        int deleted = orderRepository.DeleteOrders(new()
        {
            Month = 2,
            Year = 2026,
            Status = OrderStatus.Cancelled,
            ProductId = product1,
        });

        IReadOnlyCollection<Order> remainingOrders = orderRepository.GetOrders(new()
        {
            Month = 2,
            Year = 2026,
            Status = OrderStatus.Cancelled,
        });

        Assert.Equal(2, deleted);
        Order remainingOrder = Assert.Single(remainingOrders);
        Assert.Equal(product2, remainingOrder.ProductId);
    }
}
