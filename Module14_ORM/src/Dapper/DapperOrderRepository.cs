using System.Data;
using Dapper;
using Module14_ORM.Domain;

namespace Module14_ORM.Dapper;

public sealed class DapperOrderRepository(IDbConnectionFactory connectionFactory, IOrderStoredProcedureGateway storedProcedureGateway) : IOrderRepository
{
    public async Task<int> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        const string sql = """
            insert into Orders (Status, CreatedDate, UpdatedDate, ProductId)
            values (@Status, @CreatedDate, @UpdatedDate, @ProductId);
            select last_insert_rowid();
            """;

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        long id = await connection.QuerySingleAsync<long>(new CommandDefinition(sql, order, cancellationToken: cancellationToken));
        order.Id = checked((int)id);
        return order.Id;
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "select Id, Status, CreatedDate, UpdatedDate, ProductId from Orders where Id = @Id";

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        return await connection.QuerySingleOrDefaultAsync<Order>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "select Id, Status, CreatedDate, UpdatedDate, ProductId from Orders order by Id";

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        IEnumerable<Order> orders = await connection.QueryAsync<Order>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return orders.AsList();
    }

    public async Task<int> UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        const string sql = """
            update Orders
            set Status = @Status,
                CreatedDate = @CreatedDate,
                UpdatedDate = @UpdatedDate,
                ProductId = @ProductId
            where Id = @Id;
            """;

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        return await connection.ExecuteAsync(new CommandDefinition(sql, order, cancellationToken: cancellationToken));
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "delete from Orders where Id = @Id";

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        return await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public Task<IReadOnlyList<Order>> GetFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        return storedProcedureGateway.GetFilteredAsync(filter, cancellationToken);
    }

    public Task<int> DeleteFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        return storedProcedureGateway.DeleteFilteredAsync(filter, cancellationToken);
    }

    private static void EnsureOpen(IDbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }
}