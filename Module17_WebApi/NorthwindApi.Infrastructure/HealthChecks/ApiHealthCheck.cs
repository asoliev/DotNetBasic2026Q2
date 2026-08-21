using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NorthwindApi.HealthChecks;

public sealed class ApiHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default) => Task.FromResult(HealthCheckResult.Healthy("API is running."));
}
