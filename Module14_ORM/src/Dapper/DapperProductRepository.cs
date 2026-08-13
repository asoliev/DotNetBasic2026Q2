using System.Data;
using Dapper;
using Module14_ORM.Domain;

namespace Module14_ORM.Dapper;

public sealed class DapperProductRepository(IDbConnectionFactory connectionFactory) : IProductRepository
{
    public async Task<int> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = """
            insert into Products (Name, Description, Weight, Height, Width, Length)
            values (@Name, @Description, @Weight, @Height, @Width, @Length);
            select last_insert_rowid();
            """;

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        long id = await connection.QuerySingleAsync<long>(new CommandDefinition(sql, product, cancellationToken: cancellationToken));
        product.Id = checked((int)id);
        return product.Id;
    }

    public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "select Id, Name, Description, Weight, Height, Width, Length from Products where Id = @Id";

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        return await connection.QuerySingleOrDefaultAsync<Product>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        const string sql = "select Id, Name, Description, Weight, Height, Width, Length from Products order by Id";

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        IEnumerable<Product> products = await connection.QueryAsync<Product>(new CommandDefinition(sql, cancellationToken: cancellationToken));
        return products.AsList();
    }

    public async Task<int> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        const string sql = """
            update Products
            set Name = @Name,
                Description = @Description,
                Weight = @Weight,
                Height = @Height,
                Width = @Width,
                Length = @Length
            where Id = @Id;
            """;

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        return await connection.ExecuteAsync(new CommandDefinition(sql, product, cancellationToken: cancellationToken));
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        const string sql = "delete from Products where Id = @Id";

        using IDbConnection connection = connectionFactory.CreateConnection();
        EnsureOpen(connection);

        return await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    private static void EnsureOpen(IDbConnection connection)
    {
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }
}