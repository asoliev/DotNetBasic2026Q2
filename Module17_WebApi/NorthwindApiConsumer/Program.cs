using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using NorthwindApi.Contracts;

string baseUrl = args.Length > 0
    ? args[0]
    : ReadBaseUrlFromAppSettings();

using HttpClient client = new()
{
    BaseAddress = new Uri(baseUrl)
};

JsonSerializerOptions jsonOptions = new()
{
    WriteIndented = true
};

Console.WriteLine($"Northwind API consumer started against {baseUrl}");

await RunReadDemoAsync();
await RunMutationDemoAsync();

async Task RunReadDemoAsync()
{
    await GetAndPrintAsync("/api/categories", "Categories", jsonOptions);
    await GetAndPrintAsync("/api/products", "Products page 1", jsonOptions);
    await GetAndPrintAsync("/api/products?pageNumber=2&pageSize=5", "Products page 2 size 5", jsonOptions);
    await GetAndPrintAsync("/api/products?categoryId=1&pageNumber=1&pageSize=5", "Products filtered by category 1", jsonOptions);
    await GetAndPrintAsync("/api/products/1", "Product 1", jsonOptions);
}

async Task GetAndPrintAsync(string requestUri, string title, JsonSerializerOptions options)
{
    using HttpResponseMessage response = await client.GetAsync(requestUri);
    string content = await response.Content.ReadAsStringAsync();

    Console.WriteLine();
    Console.WriteLine($"=== {title} ===");
    Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");

    PrintPaginationHeaders(response);

    if (!string.IsNullOrWhiteSpace(content))
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(content);
            string pretty = JsonSerializer.Serialize(document.RootElement, options);
            Console.WriteLine(pretty);
        }
        catch (JsonException)
        {
            Console.WriteLine(content);
        }
    }
}

async Task RunMutationDemoAsync()
{
    Console.WriteLine();
    Console.WriteLine("=== Mutation demo ===");

    string stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
    string categoryName = $"Task3 Category {stamp}";
    string productName = $"Task3 Product {stamp}";

    CategoryDto createdCategory = await PostJsonAsync<CategoryDto>(
        "/api/categories",
        new { categoryName, description = "Temporary category for Task 3 consumer demo" },
        "Create category");

    await PutAsync(
        $"/api/categories/{createdCategory.CategoryId}",
        new { categoryName = $"{categoryName} Updated", description = "Updated temporary category" },
        "Update category");

    await GetAndPrintAsync($"/api/categories/{createdCategory.CategoryId}", "Updated category", jsonOptions);

    ProductDto createdProduct = await PostJsonAsync<ProductDto>(
        "/api/products",
        new
        {
            productName,
            supplierId = 1,
            categoryId = createdCategory.CategoryId,
            quantityPerUnit = "1 box",
            unitPrice = 12.34m,
            unitsInStock = (short)7,
            unitsOnOrder = (short)0,
            reorderLevel = (short)1,
            discontinued = false
        },
        "Create product");

    await PutAsync(
        $"/api/products/{createdProduct.ProductId}",
        new
        {
            productName = $"{productName} Updated",
            supplierId = 1,
            categoryId = createdCategory.CategoryId,
            quantityPerUnit = "2 boxes",
            unitPrice = 23.45m,
            unitsInStock = (short)5,
            unitsOnOrder = (short)1,
            reorderLevel = (short)1,
            discontinued = false
        },
        "Update product");

    await GetAndPrintAsync($"/api/products/{createdProduct.ProductId}", "Updated product", jsonOptions);

    await DeleteAsync($"/api/products/{createdProduct.ProductId}", "Delete product");
    await DeleteAsync($"/api/categories/{createdCategory.CategoryId}", "Delete category");
}

async Task<T> PostJsonAsync<T>(string requestUri, object body, string title)
{
    using HttpResponseMessage response = await client.PostAsJsonAsync(requestUri, body);
    string content = await response.Content.ReadAsStringAsync();
    PrintMutationResult(title, response, content);
    response.EnsureSuccessStatusCode();

    return JsonSerializer.Deserialize<T>(content, jsonOptions)
        ?? throw new InvalidOperationException($"Could not deserialize {title} response.");
}

async Task PutAsync(string requestUri, object body, string title)
{
    using HttpResponseMessage response = await client.PutAsJsonAsync(requestUri, body);
    string content = await response.Content.ReadAsStringAsync();
    PrintMutationResult(title, response, content);
    response.EnsureSuccessStatusCode();
}

async Task DeleteAsync(string requestUri, string title)
{
    using HttpResponseMessage response = await client.DeleteAsync(requestUri);
    string content = await response.Content.ReadAsStringAsync();
    PrintMutationResult(title, response, content);
    response.EnsureSuccessStatusCode();
}

void PrintMutationResult(string title, HttpResponseMessage response, string content)
{
    Console.WriteLine();
    Console.WriteLine($"--- {title} ---");
    Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");

    if (!string.IsNullOrWhiteSpace(content))
        Console.WriteLine(content);
}

void PrintPaginationHeaders(HttpResponseMessage response)
{
    PrintHeader(response, "X-Total-Items", "Total items");
    PrintHeader(response, "X-Total-Pages", "Total pages");
    PrintHeader(response, "X-Page-Number", "Page number");
    PrintHeader(response, "X-Page-Size", "Page size");
}

void PrintHeader(HttpResponseMessage response, string headerName, string label)
{
    if (response.Headers.TryGetValues(headerName, out IEnumerable<string>? values))
        Console.WriteLine($"{label}: {values.FirstOrDefault()}");
}

static string ReadBaseUrlFromAppSettings()
{
    string appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

    if (!File.Exists(appSettingsPath))
        return "http://127.0.0.1:5055";

    JsonNode? root = JsonNode.Parse(File.ReadAllText(appSettingsPath));
    return root?["NorthwindApi"]?["BaseUrl"]?.GetValue<string>() ?? "http://127.0.0.1:5055";
}

