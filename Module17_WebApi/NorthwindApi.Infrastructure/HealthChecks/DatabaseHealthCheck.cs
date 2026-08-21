using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NorthwindApi.HealthChecks;

public sealed class DatabaseHealthCheck(IConfiguration configuration) : IHealthCheck
{
    private readonly string connectionString = configuration.GetConnectionString("Northwind")
        ?? throw new InvalidOperationException("Connection string 'Northwind' was not found.");

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using SqlConnection connection = new(connectionString);
            await connection.OpenAsync(cancellationToken);

            await using SqlCommand command = new("SELECT 1", connection);
            _ = await command.ExecuteScalarAsync(cancellationToken);

            return HealthCheckResult.Healthy("Database is reachable.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Database is not reachable.", exception);
        }
    }
}
