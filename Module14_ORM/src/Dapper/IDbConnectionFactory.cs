using System.Data;

namespace Module14_ORM.Dapper;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public sealed class SqliteConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection() => new Microsoft.Data.Sqlite.SqliteConnection(connectionString);
}