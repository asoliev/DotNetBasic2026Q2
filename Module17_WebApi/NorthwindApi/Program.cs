using NorthwindApi.Repositories;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<INorthwindRepository, InMemoryNorthwindRepository>();

WebApplication app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();