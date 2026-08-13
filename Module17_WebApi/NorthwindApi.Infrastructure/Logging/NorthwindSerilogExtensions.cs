using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Expressions;

namespace NorthwindApi.Infrastructure.Logging;

public static class NorthwindSerilogExtensions
{
    public static IHostBuilder UseNorthwindSerilog(this IHostBuilder hostBuilder) =>
        hostBuilder.UseSerilog(ConfigureLogger);

    private static void ConfigureLogger(HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration)
    {
        int retainedFileCountLimit = context.Configuration.GetValue<int?>("LogFiles:RetainedFileCountLimit") ?? 14;
        long fileSizeLimitBytes = context.Configuration.GetValue<long?>("LogFiles:FileSizeLimitBytes") ?? 10 * 1024 * 1024;
        FileSinkConfigurator fileSinkConfigurator = new(fileSizeLimitBytes, retainedFileCountLimit);

        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Debug)
            .WriteTo.Logger(fileSinkConfigurator.ConfigureInfo)
            .WriteTo.Logger(fileSinkConfigurator.ConfigureWarning)
            .WriteTo.Logger(fileSinkConfigurator.ConfigureError);
    }

    private sealed class FileSinkConfigurator(long fileSizeLimitBytes, int retainedFileCountLimit)
    {
        public void ConfigureInfo(LoggerConfiguration loggerConfiguration) =>
            ConfigureInfoFileSink(loggerConfiguration, fileSizeLimitBytes, retainedFileCountLimit);

        public void ConfigureWarning(LoggerConfiguration loggerConfiguration) =>
            ConfigureWarningFileSink(loggerConfiguration, fileSizeLimitBytes, retainedFileCountLimit);

        public void ConfigureError(LoggerConfiguration loggerConfiguration) =>
            ConfigureErrorFileSink(loggerConfiguration, fileSizeLimitBytes, retainedFileCountLimit);
    }

    private static LoggerConfiguration ConfigureInfoFileSink(LoggerConfiguration loggerConfiguration, long fileSizeLimitBytes, int retainedFileCountLimit) => loggerConfiguration
            .Filter.ByIncludingOnly("@l = 'Verbose' or @l = 'Debug' or @l = 'Information'")
            .WriteTo.File(
                path: "logs/northwind-api-info-.log",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: fileSizeLimitBytes,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: retainedFileCountLimit,
                restrictedToMinimumLevel: LogEventLevel.Verbose,
                shared: true);

    private static LoggerConfiguration ConfigureWarningFileSink(LoggerConfiguration loggerConfiguration, long fileSizeLimitBytes, int retainedFileCountLimit) => loggerConfiguration
            .Filter.ByIncludingOnly("@l = 'Warning'")
            .WriteTo.File(
                path: "logs/northwind-api-warning-.log",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: fileSizeLimitBytes,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: retainedFileCountLimit,
                restrictedToMinimumLevel: LogEventLevel.Warning,
                shared: true);

    private static LoggerConfiguration ConfigureErrorFileSink(LoggerConfiguration loggerConfiguration, long fileSizeLimitBytes, int retainedFileCountLimit) => loggerConfiguration
            .Filter.ByIncludingOnly("@l = 'Error' or @l = 'Fatal'")
            .WriteTo.File(
                path: "logs/northwind-api-errors-.log",
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: fileSizeLimitBytes,
                rollOnFileSizeLimit: true,
                retainedFileCountLimit: retainedFileCountLimit,
                restrictedToMinimumLevel: LogEventLevel.Error,
                shared: true);
}