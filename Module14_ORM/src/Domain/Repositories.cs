namespace Module14_ORM.Domain;

public interface IProductRepository
{
    Task<int> CreateAsync(Product product, CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<int> UpdateAsync(Product product, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public interface IOrderRepository
{
    Task<int> CreateAsync(Order order, CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<int> UpdateAsync(Order order, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Order>> GetFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default);

    Task<int> DeleteFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default);
}

public interface IOrderStoredProcedureGateway
{
    Task<IReadOnlyList<Order>> GetFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default);

    Task<int> DeleteFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default);
}