using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindApi.Contracts;
using NorthwindApi.Controllers;
using NorthwindApi.HealthChecks;
using NorthwindApi.Infrastructure;
using NorthwindApi.Infrastructure.Logging;
using NorthwindApi.Models;
using NorthwindApi.Repositories.Categories;
using NorthwindApi.Repositories.Products;
using NorthwindApi.Middleware;
using NorthwindApi.Services.Categories;
using NorthwindApi.Services.Common;
using NorthwindApi.Services.Products;

namespace NorthwindApiConsumer.Tests;

[TestClass]
public sealed class HostLayerTests
{
    [TestMethod]
    public void AddNorthwindApiServices_RegistersExpectedServices()
    {
        ServiceCollection services = [];

        IServiceCollection returned = services.AddNorthwindApiServices();

        Assert.AreSame(services, returned);
        Assert.IsTrue(services.Any(descriptor => descriptor.ServiceType == typeof(ICategoryRepository) && descriptor.ImplementationType == typeof(SqlCategoryRepository)));
        Assert.IsTrue(services.Any(descriptor => descriptor.ServiceType == typeof(IProductRepository) && descriptor.ImplementationType == typeof(SqlProductRepository)));
        Assert.IsTrue(services.Any(descriptor => descriptor.ServiceType == typeof(ICategoryService) && descriptor.ImplementationType == typeof(CategoryService)));
        Assert.IsTrue(services.Any(descriptor => descriptor.ServiceType == typeof(IProductService) && descriptor.ImplementationType == typeof(ProductService)));
    }

    [TestMethod]
    public void ServiceResultControllerExtensions_ReturnExpectedActionResults()
    {
        DummyController controller = new();

        ActionResult<Category> getResult = controller.ToGetActionResult(ServiceResult<Category>.Success(new Category { CategoryId = 1, CategoryName = "Seafood" }));
        ActionResult<Category> getNotFound = controller.ToGetActionResult(ServiceResult<Category>.NotFound());
        IActionResult mutationResult = controller.ToMutationActionResult(ServiceResult<bool>.Success(true));
        IActionResult mutationConflict = controller.ToMutationActionResult(ServiceResult<bool>.Conflict("Blocked"));
        ActionResult<Category> createdResult = controller.ToCreatedActionResult(ServiceResult<Category>.Success(new Category { CategoryId = 2, CategoryName = "Beverages" }), nameof(DummyController.GetById), new { id = 2 });

        Assert.IsInstanceOfType(getResult.Result, typeof(OkObjectResult));
        Assert.IsInstanceOfType(getNotFound.Result, typeof(NotFoundResult));
        Assert.IsInstanceOfType(mutationResult, typeof(NoContentResult));
        Assert.IsInstanceOfType(mutationConflict, typeof(ConflictObjectResult));
        Assert.IsInstanceOfType(createdResult.Result, typeof(CreatedAtActionResult));
    }

    [TestMethod]
    public void CategoriesController_ReturnsExpectedResponses()
    {
        FakeCategoryService service = new();
        CategoriesController controller = new(service);

        ActionResult<IReadOnlyList<Category>> getAll = controller.GetAll();
        ActionResult<Category> getById = controller.GetById(1);
        ActionResult<Category> created = controller.Create(new CategoryUpsertRequest { CategoryName = "Drinks" });
        IActionResult updated = controller.Update(1, new CategoryUpsertRequest { CategoryName = "Seafood Updated" });
        IActionResult deleted = controller.Delete(1);

        Assert.IsInstanceOfType(getAll.Result, typeof(OkObjectResult));
        Assert.IsInstanceOfType(getById.Result, typeof(OkObjectResult));
        Assert.IsInstanceOfType(created.Result, typeof(CreatedAtActionResult));
        Assert.IsInstanceOfType(updated, typeof(NoContentResult));
        Assert.IsInstanceOfType(deleted, typeof(NoContentResult));
    }

    [TestMethod]
    public void ProductsController_SetsPaginationHeadersAndReturnsResults()
    {
        FakeProductService service = new();
        ProductsController controller = new(service)
        {
            ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() }
        };

        ActionResult<IReadOnlyList<Product>> getAll = controller.GetAll(2, 5, 1);
        ActionResult<Product> getById = controller.GetById(1);
        ActionResult<Product> created = controller.Create(new ProductUpsertRequest { ProductName = "Coffee", CategoryId = 1, SupplierId = 1 });
        IActionResult updated = controller.Update(1, new ProductUpsertRequest { ProductName = "Coffee Updated", CategoryId = 1, SupplierId = 1 });
        IActionResult deleted = controller.Delete(1);

        Assert.IsInstanceOfType(getAll.Result, typeof(OkObjectResult));
            Assert.AreEqual("2", controller.Response.Headers["X-Page-Number"].ToString());
            Assert.AreEqual("5", controller.Response.Headers["X-Page-Size"].ToString());
            Assert.AreEqual("1", controller.Response.Headers["X-Total-Items"].ToString());
        OkObjectResult getAllPayload = (OkObjectResult)getAll.Result!;
        IReadOnlyList<Product> products = (IReadOnlyList<Product>)getAllPayload.Value!;
        Assert.AreEqual(1, products.Count);
        Assert.IsInstanceOfType(getById.Result, typeof(OkObjectResult));
        Assert.IsInstanceOfType(created.Result, typeof(CreatedAtActionResult));
        Assert.IsInstanceOfType(updated, typeof(NoContentResult));
        Assert.IsInstanceOfType(deleted, typeof(NoContentResult));
    }

    [TestMethod]
    public async Task HealthCheckResponseWriter_WritesStructuredJson()
    {
        DefaultHttpContext context = new();
        context.Response.Body = new MemoryStream();

        HealthReport report = new(
            new Dictionary<string, HealthReportEntry>
            {
                ["api"] = new HealthReportEntry(HealthStatus.Healthy, "API is running.", TimeSpan.Zero, null, null)
            },
            TimeSpan.FromMilliseconds(12));

        await HealthCheckResponseWriter.WriteAsync(context, report);

        context.Response.Body.Position = 0;
        string json = await new StreamReader(context.Response.Body, Encoding.UTF8).ReadToEndAsync();

        Assert.IsTrue(json.Contains("\"status\": \"Healthy\""));
        Assert.IsTrue(json.Contains("\"name\": \"api\""));
        Assert.AreEqual("application/json; charset=utf-8", context.Response.ContentType);
    }

    [TestMethod]
    public async Task ApiHealthCheck_ReturnsHealthy()
    {
        HealthCheckResult result = await new ApiHealthCheck().CheckHealthAsync(new HealthCheckContext());

        Assert.AreEqual(HealthStatus.Healthy, result.Status);
        Assert.AreEqual("API is running.", result.Description);
    }

    [TestMethod]
    public async Task DatabaseHealthCheck_ReturnsHealthyAgainstNorthwind()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Northwind"] = "Server=localhost,1433;Database=Northwind;User Id=sa;Password=YourStrongP@ssw0rd!;Encrypt=True;TrustServerCertificate=True"
            })
            .Build();

        HealthCheckResult result = await new DatabaseHealthCheck(configuration).CheckHealthAsync(new HealthCheckContext());

        Assert.AreEqual(HealthStatus.Healthy, result.Status);
    }

    [TestMethod]
    public void MapNorthwindHealthChecks_RegistersEndpoints()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        builder.Services.AddHealthChecks();
        WebApplication app = builder.Build();

        IEndpointRouteBuilder returned = app.MapNorthwindHealthChecks();

        Assert.AreSame(app, returned);
    }

    [TestMethod]
    public async Task UseNorthwindRequestLogging_HandlesSuccessAndFailure()
    {
        await ExerciseRequestLogging(success: true);
        await ExerciseRequestLogging(success: false);
    }

    [TestMethod]
    public void UseNorthwindSerilog_BuildsHostSuccessfully()
    {
        using IHost host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["LogFiles:RetainedFileCountLimit"] = "1",
                ["LogFiles:FileSizeLimitBytes"] = "1024"
            }))
            .UseNorthwindSerilog()
            .Build();

        Assert.IsNotNull(host);
    }

    private sealed class DummyController : ControllerBase
    {
        public ActionResult<Category> GetById(int id) => new Category { CategoryId = id, CategoryName = "Dummy" };
    }

    private static async Task ExerciseRequestLogging(bool success)
    {
        ServiceCollection services = [];
        services.AddSingleton<ILoggerFactory>(_ => new LoggerFactory([new AlwaysEnabledLoggerProvider()]));
        IServiceProvider serviceProvider = services.BuildServiceProvider();

        ApplicationBuilder app = new(serviceProvider);
        ILogger logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Test");

        app.UseNorthwindRequestLogging(logger);
        app.Run(async context =>
        {
            if (!success)
                throw new InvalidOperationException("boom");

            context.Response.StatusCode = StatusCodes.Status200OK;
            await context.Response.WriteAsync("ok");
        });

        RequestDelegate pipeline = app.Build();
        DefaultHttpContext context = new();

        if (success)
        {
            await pipeline(context);
            return;
        }

        try
        {
            await pipeline(context);
            Assert.Fail("Expected the pipeline to throw.");
        }
        catch (InvalidOperationException)
        {
        }
    }

    private sealed class AlwaysEnabledLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new AlwaysEnabledLogger();

        public void Dispose()
        {
        }
    }

    private sealed class AlwaysEnabledLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();

            public void Dispose()
            {
            }
        }
    }

    private sealed class FakeCategoryService : ICategoryService
    {
        private readonly List<Category> categories = [new() { CategoryId = 1, CategoryName = "Seafood" }];

        public IReadOnlyList<Category> GetAll() => categories;

        public ServiceResult<Category> GetById(int id)
        {
            Category? category = categories.FirstOrDefault(category => category.CategoryId == id);
            return category is null ? ServiceResult<Category>.NotFound() : ServiceResult<Category>.Success(category);
        }

        public ServiceResult<Category> Create(CategoryUpsertRequest request)
        {
            Category category = new() { CategoryId = 2, CategoryName = request.CategoryName, Description = request.Description };
            categories.Add(category);
            return ServiceResult<Category>.Success(category);
        }

        public ServiceResult<bool> Update(int id, CategoryUpsertRequest request)
            => categories.Any(category => category.CategoryId == id)
                ? ServiceResult<bool>.Success(true)
                : ServiceResult<bool>.NotFound();

        public ServiceResult<bool> Delete(int id)
            => categories.RemoveAll(category => category.CategoryId == id) > 0
                ? ServiceResult<bool>.Success(true)
                : ServiceResult<bool>.NotFound();
    }

    private sealed class FakeProductService : IProductService
    {
        private readonly List<Product> products = [new() { ProductId = 1, ProductName = "Chai", CategoryId = 1, SupplierId = 1 }];

        public PagedResult<Product> GetAll(int pageNumber = 1, int pageSize = 10, int? categoryId = null)
            => new(products, products.Count, pageNumber, pageSize);

        public ServiceResult<Product> GetById(int id)
        {
            Product? product = products.FirstOrDefault(product => product.ProductId == id);
            return product is null ? ServiceResult<Product>.NotFound() : ServiceResult<Product>.Success(product);
        }

        public ServiceResult<Product> Create(ProductUpsertRequest request)
        {
            Product product = new() { ProductId = 2, ProductName = request.ProductName, CategoryId = request.CategoryId, SupplierId = request.SupplierId };
            products.Add(product);
            return ServiceResult<Product>.Success(product);
        }

        public ServiceResult<bool> Update(int id, ProductUpsertRequest request)
            => products.Any(product => product.ProductId == id)
                ? ServiceResult<bool>.Success(true)
                : ServiceResult<bool>.NotFound();

        public ServiceResult<bool> Delete(int id)
            => products.RemoveAll(product => product.ProductId == id) > 0
                ? ServiceResult<bool>.Success(true)
                : ServiceResult<bool>.NotFound();
    }
}