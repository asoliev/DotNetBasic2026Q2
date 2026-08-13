using System.Net.Http.Json;
using System.Xml.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindApi.Contracts;

namespace NorthwindApiConsumer.Tests;

[TestClass]
public sealed class ApiConsumerTests
{
    private static HttpClient client = null!;
    private static JsonSerializerOptions jsonOptions = null!;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
        string baseUrl = ReadBaseUrlFromRunSettings();

        client = new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        };

        jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        context.WriteLine($"API consumer tests started against {baseUrl}");
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        client.Dispose();
    }

    [TestMethod]
    public async Task GetCategories_ReturnsSuccess()
        => await AssertSuccessStatusCodeAsync("/api/categories");

    [TestMethod]
    public async Task GetPagedProducts_ReturnsPaginationHeaders()
    {
        using HttpResponseMessage response = await client.GetAsync("/api/products?pageNumber=1&pageSize=5");

        Assert.IsTrue(response.IsSuccessStatusCode);
        AssertPaginationHeaders(response);
    }

    [TestMethod]
    public async Task MutationFlow_CreatesUpdatesAndDeletesTemporaryRecords()
    {
        string stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        string categoryName = $"C{stamp[^6..]}";
        string productName = $"P{stamp}";

        CategoryDto createdCategory = await PostAsync<CategoryDto>("/api/categories", new
        {
            categoryName,
            description = "Temporary category created by MSTest consumer"
        });

        await PutAsync($"/api/categories/{createdCategory.CategoryId}", new
        {
            categoryName = $"{categoryName} Updated",
            description = "Updated temporary category"
        });

        CategoryDto fetchedCategory = await GetAsync<CategoryDto>($"/api/categories/{createdCategory.CategoryId}");
        Assert.AreEqual($"{categoryName} Updated", fetchedCategory.CategoryName);

        ProductDto createdProduct = await PostAsync<ProductDto>("/api/products", new
        {
            productName,
            supplierId = 1,
            categoryId = createdCategory.CategoryId,
            quantityPerUnit = "1 box",
            unitPrice = 11.11m,
            unitsInStock = (short)4,
            unitsOnOrder = (short)0,
            reorderLevel = (short)1,
            discontinued = false
        });

        await PutAsync($"/api/products/{createdProduct.ProductId}", new
        {
            productName = $"{productName} Updated",
            supplierId = 1,
            categoryId = createdCategory.CategoryId,
            quantityPerUnit = "2 boxes",
            unitPrice = 22.22m,
            unitsInStock = (short)3,
            unitsOnOrder = (short)1,
            reorderLevel = (short)1,
            discontinued = false
        });

        ProductDto fetchedProduct = await GetAsync<ProductDto>($"/api/products/{createdProduct.ProductId}");
        Assert.AreEqual($"{productName} Updated", fetchedProduct.ProductName);

        await AssertSuccessStatusCodeAsync($"/api/products/{createdProduct.ProductId}", HttpMethod.Delete);
        await AssertSuccessStatusCodeAsync($"/api/categories/{createdCategory.CategoryId}", HttpMethod.Delete);
    }

    private static async Task AssertSuccessStatusCodeAsync(string requestUri, HttpMethod? method = null)
    {
        using HttpRequestMessage request = new(method ?? HttpMethod.Get, requestUri);
        using HttpResponseMessage response = await client.SendAsync(request);

        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    private static void AssertPaginationHeaders(HttpResponseMessage response)
    {
        Assert.IsTrue(response.Headers.Contains("X-Total-Items"));
        Assert.IsTrue(response.Headers.Contains("X-Total-Pages"));
        Assert.IsTrue(response.Headers.Contains("X-Page-Number"));
        Assert.IsTrue(response.Headers.Contains("X-Page-Size"));
    }

    private static async Task<T> PostAsync<T>(string requestUri, object body)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, body, jsonOptions);
        string content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<T>(content, jsonOptions)!
            ?? throw new InvalidOperationException("Could not deserialize POST response.");
    }

    private static async Task PutAsync(string requestUri, object body)
    {
        using HttpResponseMessage response = await client.PutAsJsonAsync(requestUri, body, jsonOptions);
        response.EnsureSuccessStatusCode();
    }

    private static async Task<T> GetAsync<T>(string requestUri)
    {
        using HttpResponseMessage response = await client.GetAsync(requestUri);
        string content = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<T>(content, jsonOptions)!
            ?? throw new InvalidOperationException("Could not deserialize GET response.");
    }

    private static string ReadBaseUrlFromRunSettings()
    {
        string runSettingsPath = Path.Combine(AppContext.BaseDirectory, "NorthwindApiConsumer.runsettings");

        if (!File.Exists(runSettingsPath))
            return "http://127.0.0.1:5055";

        XDocument document = XDocument.Load(runSettingsPath);
        XElement? parameter = document
            .Descendants("Parameter")
            .FirstOrDefault(element => string.Equals((string?)element.Attribute("name"), "NorthwindApiBaseUrl", StringComparison.OrdinalIgnoreCase));

        return parameter?.Attribute("value")?.Value ?? "http://127.0.0.1:5055";
    }
}