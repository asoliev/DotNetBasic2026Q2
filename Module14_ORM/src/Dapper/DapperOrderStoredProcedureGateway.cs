using System.Data;
using Dapper;
using Module14_ORM.Domain;

namespace Module14_ORM.Dapper;

public sealed class DapperOrderStoredProcedureGateway(IDbConnectionFactory connectionFactory) : IOrderStoredProcedureGateway
{
    private const string GetOrdersProcedure = "dbo.usp_Orders_GetFiltered";
    private const string DeleteOrdersProcedure = "dbo.usp_Orders_DeleteFiltered";

    public async Task<IReadOnlyList<Order>> GetFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        IEnumerable<Order> orders = await connection.QueryAsync<Order>(new CommandDefinition(
            GetOrdersProcedure,
            BuildParameters(filter),
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));

        return orders.AsList();
    }

    public async Task<int> DeleteFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        return await connection.ExecuteAsync(new CommandDefinition(
            DeleteOrdersProcedure,
            BuildParameters(filter),
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }

    private static DynamicParameters BuildParameters(OrderFilter filter)
    {
        DynamicParameters parameters = new();
        parameters.Add("Month", filter.Month);
        parameters.Add("Status", filter.Status);
        parameters.Add("Year", filter.Year);
        parameters.Add("ProductId", filter.ProductId);
        return parameters;
    }

    private static void EnsureOpen(IDbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }
}