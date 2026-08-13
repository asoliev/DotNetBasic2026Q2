using NorthwindApi.HealthChecks;
using NorthwindApi.Repositories.Categories;
using NorthwindApi.Repositories.Products;
using NorthwindApi.Services.Categories;
using NorthwindApi.Services.Products;

namespace NorthwindApi.Infrastructure;

public static class NorthwindApiServiceCollectionExtensions
{
    public static IServiceCollection AddNorthwindApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddSingleton<ICategoryRepository, SqlCategoryRepository>();
        services.AddSingleton<IProductRepository, SqlProductRepository>();
        services.AddSingleton<ICategoryService, CategoryService>();
        services.AddSingleton<IProductService, ProductService>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddHealthChecks()
	        .AddCheck<ApiHealthCheck>("api")
	        .AddCheck<DatabaseHealthCheck>("db");

        return services;
    }
}