using NorthwindApi.HealthChecks;
using NorthwindApi.Infrastructure.Logging;
using NorthwindApi.Infrastructure;
using NorthwindApi.Middleware;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseNorthwindSerilog();
builder.Services.AddNorthwindApiServices();

WebApplication app = builder.Build();

ILogger<Program> logger = app.Services.GetRequiredService<ILogger<Program>>();

app.UseHttpsRedirection();
app.UseNorthwindRequestLogging(logger);

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => Results.Text("Northwind API is running", "text/plain"));

app.MapNorthwindHealthChecks();

app.MapControllers();

logger.LogDebug("Northwind API started.");

app.Run();
