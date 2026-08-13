using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NorthwindApi.Middleware;

public static class NorthwindRequestLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseNorthwindRequestLogging(this IApplicationBuilder app, ILogger logger) =>
        app.Use((context, next) => LogRequestAsync(context, next, logger));

    private static async Task LogRequestAsync(HttpContext context, Func<Task> next, ILogger logger)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        try
        {
            await next();
        }
        catch (Exception exception)
        {
            stopwatch.Stop();
            LogRequestFailure(logger, context, exception, stopwatch.ElapsedMilliseconds);
            throw;
        }

        stopwatch.Stop();

        LogLevel logLevel = GetLogLevel(context.Response.StatusCode);
        LogRequestCompletion(logger, context, stopwatch.ElapsedMilliseconds, logLevel);
    }

    private static void LogRequestFailure(ILogger logger, HttpContext context, Exception exception, long elapsedMilliseconds)
    {
        if (!logger.IsEnabled(LogLevel.Error))
            return;

        logger.LogError(exception,
            "HTTP {Method} {Path} failed after {ElapsedMilliseconds} ms.",
            context.Request.Method,
            context.Request.Path,
            elapsedMilliseconds);
    }

    private static void LogRequestCompletion(ILogger logger, HttpContext context, long elapsedMilliseconds, LogLevel logLevel)
    {
        if (!logger.IsEnabled(logLevel))
            return;

        logger.Log(logLevel,
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms.",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            elapsedMilliseconds);
    }

    private static LogLevel GetLogLevel(int statusCode) => statusCode switch
    {
        >= 500 => LogLevel.Error,
        >= 400 => LogLevel.Warning,
        200 => LogLevel.Information,
        _ => LogLevel.Debug
    };
}
