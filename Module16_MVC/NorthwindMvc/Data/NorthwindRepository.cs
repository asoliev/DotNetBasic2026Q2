using Microsoft.Data.SqlClient;
using NorthwindMvc.Models;

namespace NorthwindMvc.Data;

public sealed class NorthwindRepository
{
    private const string ConnectionString = "Server=localhost;Database=Northwind;Trusted_Connection=True;TrustServerCertificate=True";

    public async Task<IReadOnlyList<CategoryListItem>> GetCategoriesAsync()
    {
        List<CategoryListItem> categories = [];

        await using SqlConnection connection = new(ConnectionString);
        await connection.OpenAsync();

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT CategoryID, CategoryName, Description
            FROM Categories
            ORDER BY CategoryName;
            """;

        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categories.Add(new CategoryListItem(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? null : reader.GetString(2)));
        }

        return categories;
    }

    public async Task<IReadOnlyList<ProductListItem>> GetProductsAsync()
    {
        List<ProductListItem> products = [];

        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT
                P.ProductID,
                P.ProductName,
                S.CompanyName AS SupplierName,
                C.CategoryName,
                P.QuantityPerUnit,
                P.UnitPrice,
                P.UnitsInStock,
                P.UnitsOnOrder,
                P.ReorderLevel,
                P.Discontinued
            FROM Products AS P
            INNER JOIN Suppliers AS S ON S.SupplierID = P.SupplierID
            LEFT JOIN Categories AS C ON C.CategoryID = P.CategoryID
            ORDER BY P.ProductName;
            """;

        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new ProductListItem(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.IsDBNull(4) ? null : reader.GetString(4),
                reader.IsDBNull(5) ? null : reader.GetDecimal(5),
                reader.IsDBNull(6) ? null : reader.GetInt16(6),
                reader.IsDBNull(7) ? null : reader.GetInt16(7),
                reader.IsDBNull(8) ? null : reader.GetInt16(8),
                reader.GetBoolean(9)));
        }

        return products;
    }
}