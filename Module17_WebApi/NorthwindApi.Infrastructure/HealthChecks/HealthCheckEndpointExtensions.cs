using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace NorthwindApi.HealthChecks;

public static class HealthCheckEndpointExtensions
{
    public static IEndpointRouteBuilder MapNorthwindHealthChecks(this IEndpointRouteBuilder endpoints)
    {
        HealthCheckOptions allHealthChecksOptions = new()
        {
            ResponseWriter = HealthCheckResponseWriter.WriteAsync
        };

        endpoints.MapHealthChecks("/health", allHealthChecksOptions);
        endpoints.MapHealthChecks("/health/api", new HealthCheckOptions
        {
            Predicate = check => check.Name == "api",
            ResponseWriter = HealthCheckResponseWriter.WriteAsync
        });

        endpoints.MapHealthChecks("/health/db", new HealthCheckOptions
        {
            Predicate = check => check.Name == "db",
            ResponseWriter = HealthCheckResponseWriter.WriteAsync
        });

        return endpoints;
    }
}