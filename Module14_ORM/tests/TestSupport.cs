using System.Data;
using Microsoft.Data.Sqlite;
using Module14_ORM.Domain;
using Module14_ORM.Dapper;

namespace Module14_ORM.Tests;

internal sealed class SqliteTestDatabase : IAsyncDisposable
{
    private readonly SqliteConnection keeperConnection;

    private SqliteTestDatabase(string connectionString, SqliteConnection keeperConnection)
    {
        ConnectionFactory = new SqliteConnectionFactory(connectionString);
        this.keeperConnection = keeperConnection;
    }

    public IDbConnectionFactory ConnectionFactory { get; }

    public static async Task<SqliteTestDatabase> CreateAsync()
    {
        string connectionString = $"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared";
        SqliteConnection keeperConnection = new(connectionString);
        await keeperConnection.OpenAsync();

        await using SqliteCommand command = keeperConnection.CreateCommand();
        command.CommandText = """
            pragma foreign_keys = on;

            create table Products (
                Id integer primary key autoincrement,
                Name text not null,
                Description text null,
                Weight numeric not null,
                Height numeric not null,
                Width numeric not null,
                Length numeric not null
            );

            create table Orders (
                Id integer primary key autoincrement,
                Status integer not null,
                CreatedDate text not null,
                UpdatedDate text not null,
                ProductId integer not null,
                foreign key (ProductId) references Products (Id) on delete restrict
            );
            """;
        await command.ExecuteNonQueryAsync();

        return new SqliteTestDatabase(connectionString, keeperConnection);
    }

    public ValueTask DisposeAsync()
    {
        return keeperConnection.DisposeAsync();
    }
}

internal sealed class RecordingOrderStoredProcedureGateway : IOrderStoredProcedureGateway
{
    public OrderFilter? LastFilter { get; private set; }

    public IReadOnlyList<Order> OrdersToReturn { get; set; } = Array.Empty<Order>();

    public int RowsAffectedToReturn { get; set; }

    public Task<IReadOnlyList<Order>> GetFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        LastFilter = filter;
        return Task.FromResult(OrdersToReturn);
    }

    public Task<int> DeleteFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        LastFilter = filter;
        return Task.FromResult(RowsAffectedToReturn);
    }
}