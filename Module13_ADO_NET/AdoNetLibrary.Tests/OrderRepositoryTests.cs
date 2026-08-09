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

        int productId = productRepository.Create(new()
        {
            Name = "Monitor",
            Description = "24-inch monitor",
            Weight = 3.40m,
            Height = 45.00m,
            Width = 54.00m,
            Length = 18.00m,
        });

        int orderId = orderRepository.Create(new()
        {
            ProductId = productId,
            Status = OrderStatus.InProgress,
            CreatedDate = new(2026, 06, 12, 10, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new(2026, 06, 12, 10, 0, 0, DateTimeKind.Utc),
        });

        Order? loaded = orderRepository.GetById(orderId);

        Assert.NotNull(loaded);
        Assert.Equal(productId, loaded!.ProductId);
        Assert.Equal(OrderStatus.InProgress, loaded.Status);
        Assert.Equal(loaded.CreatedDate, loaded.UpdatedDate);
        Assert.Equal(new DateTime(2026, 06, 12, 10, 0, 0, DateTimeKind.Utc), loaded.CreatedDate);
    }

    [Fact]
    public void UpdateAndDelete_ShouldModifyThenRemoveOrder()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository productRepository = new(connectionString);
        OrderRepository orderRepository = new(connectionString);

        int productId = productRepository.Create(new()
        {
            Name = "SSD",
            Description = "1TB NVMe SSD",
            Weight = 0.05m,
            Height = 0.70m,
            Width = 2.20m,
            Length = 8.00m,
        });
        int orderId = orderRepository.Create(new()
        {
            ProductId = productId,
            Status = OrderStatus.NotStarted,
            CreatedDate = new(2026, 05, 01, 8, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new(2026, 05, 01, 8, 0, 0, DateTimeKind.Utc),
        });

        bool updated = orderRepository.Update(new()
        {
            Id = orderId,
            ProductId = productId,
            Status = OrderStatus.Loading,
            CreatedDate = new(2026, 05, 01, 8, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new(2026, 05, 02, 9, 0, 0, DateTimeKind.Utc),
        });

        Order? loadedAfterUpdate = orderRepository.GetById(orderId);
        bool deleted = orderRepository.Delete(orderId);
        Order? loadedAfterDelete = orderRepository.GetById(orderId);

        Assert.True(updated);
        Assert.NotNull(loadedAfterUpdate);
        Assert.Equal(OrderStatus.Loading, loadedAfterUpdate.Status);
        Assert.Equal(new DateTime(2026, 05, 01, 8, 0, 0, DateTimeKind.Utc), loadedAfterUpdate.CreatedDate);
        Assert.Equal(new DateTime(2026, 05, 02, 9, 0, 0, DateTimeKind.Utc), loadedAfterUpdate.UpdatedDate);
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

        int product1 = productRepository.Create(new()
        {
            Name = "Product A",
            Description = "First test product",
            Weight = 1.00m,
            Height = 10.00m,
            Width = 10.00m,
            Length = 10.00m,
        });
        int product2 = productRepository.Create(new()
        {
            Name = "Product B",
            Description = "Second test product",
            Weight = 2.00m,
            Height = 12.00m,
            Width = 12.00m,
            Length = 12.00m,
        });

        orderRepository.Create(new()
        {
            ProductId = product1,
            Status = OrderStatus.Arrived,
            CreatedDate = new(2026, 04, 10, 6, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new(2026, 04, 10, 6, 0, 0, DateTimeKind.Utc),
        });
        orderRepository.Create(new()
        {
            ProductId = product1,
            Status = OrderStatus.Done,
            CreatedDate = new(2026, 04, 20, 6, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new(2026, 04, 20, 6, 0, 0, DateTimeKind.Utc),
        });
        orderRepository.Create(new()
        {
            ProductId = product2,
            Status = OrderStatus.Arrived,
            CreatedDate = new(2025, 04, 10, 6, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new(2025, 04, 10, 6, 0, 0, DateTimeKind.Utc),
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
        Assert.Equal(2026, order.CreatedDate.Year);
        Assert.Equal(4, order.CreatedDate.Month);
        Assert.Equal(order.CreatedDate, order.UpdatedDate);
    }

    [Fact]
    public void DeleteOrders_ShouldBulkDeleteInTransactionByFilter()
    {
        string? connectionString = TestDatabaseHelper.GetConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString)) return;

        TestDatabaseHelper.EnsureDatabaseObjects(connectionString);
        ProductRepository productRepository = new(connectionString);
        OrderRepository orderRepository = new(connectionString);

        int product1 = productRepository.Create(new()
        {
            Name = "Product X",
            Description = "Cleanup product X",
            Weight = 1.50m,
            Height = 11.00m,
            Width = 11.00m,
            Length = 11.00m,
        });
        int product2 = productRepository.Create(new()
        {
            Name = "Product Y",
            Description = "Cleanup product Y",
            Weight = 2.50m,
            Height = 13.00m,
            Width = 13.00m,
            Length = 13.00m,
        });

        orderRepository.Create(new()
        {
            ProductId = product1,
            Status = OrderStatus.Cancelled,
            CreatedDate = new(2026, 02, 01, 7, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new(2026, 02, 01, 7, 0, 0, DateTimeKind.Utc),
        });
        orderRepository.Create(new()
        {
            ProductId = product1,
            Status = OrderStatus.Cancelled,
            CreatedDate = new(2026, 02, 15, 7, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new(2026, 02, 15, 7, 0, 0, DateTimeKind.Utc),
        });
        orderRepository.Create(new()
        {
            ProductId = product2,
            Status = OrderStatus.Cancelled,
            CreatedDate = new(2026, 02, 20, 7, 0, 0, DateTimeKind.Utc),
            UpdatedDate = new(2026, 02, 20, 7, 0, 0, DateTimeKind.Utc),
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
