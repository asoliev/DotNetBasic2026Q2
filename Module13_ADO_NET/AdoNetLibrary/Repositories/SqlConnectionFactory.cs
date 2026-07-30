using Microsoft.Data.SqlClient;

namespace AdoNetLibrary.Repositories;

public sealed class SqlConnectionFactory
{
    private readonly string connectionString;

    public SqlConnectionFactory(string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
        this.connectionString = connectionString;
    }

    public SqlConnection CreateOpenConnection()
    {
        var connection = new SqlConnection(this.connectionString);
        connection.Open();
        return connection;
    }
}
