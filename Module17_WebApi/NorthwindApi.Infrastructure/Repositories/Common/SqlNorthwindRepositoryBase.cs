using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NorthwindApi.Models;

namespace NorthwindApi.Repositories.Common;

public abstract class SqlNorthwindRepositoryBase(IConfiguration configuration)
{
    private readonly string connectionString = configuration.GetConnectionString("Northwind")
        ?? throw new InvalidOperationException("Connection string 'Northwind' was not found.");

    protected SqlConnection CreateConnection() => new(connectionString);

    protected bool Exists(string sql, string parameterName, int value)
    {
        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue(parameterName, value);
        connection.Open();

        object? result = command.ExecuteScalar();
        return result is not null;
    }

    protected static Category MapCategory(SqlDataReader reader) => new()
    {
        CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryID")),
        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName")),
        Description = reader.IsDBNull(reader.GetOrdinal("Description"))
            ? null
            : reader.GetString(reader.GetOrdinal("Description"))
    };

    protected static Product MapProduct(SqlDataReader reader)
    {
        int categoryIdOrdinal = reader.GetOrdinal("CategoryID");
        int supplierIdOrdinal = reader.GetOrdinal("SupplierID");
        int unitPriceOrdinal = reader.GetOrdinal("UnitPrice");
        int unitsInStockOrdinal = reader.GetOrdinal("UnitsInStock");
        int unitsOnOrderOrdinal = reader.GetOrdinal("UnitsOnOrder");
        int reorderLevelOrdinal = reader.GetOrdinal("ReorderLevel");

        return new Product
        {
            ProductId = reader.GetInt32(reader.GetOrdinal("ProductID")),
            ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
            SupplierId = reader.IsDBNull(supplierIdOrdinal) ? null : reader.GetInt32(supplierIdOrdinal),
            CategoryId = reader.IsDBNull(categoryIdOrdinal) ? null : reader.GetInt32(categoryIdOrdinal),
            QuantityPerUnit = reader.IsDBNull(reader.GetOrdinal("QuantityPerUnit"))
                ? null
                : reader.GetString(reader.GetOrdinal("QuantityPerUnit")),
            UnitPrice = reader.IsDBNull(unitPriceOrdinal) ? null : reader.GetDecimal(unitPriceOrdinal),
            UnitsInStock = reader.IsDBNull(unitsInStockOrdinal) ? null : reader.GetInt16(unitsInStockOrdinal),
            UnitsOnOrder = reader.IsDBNull(unitsOnOrderOrdinal) ? null : reader.GetInt16(unitsOnOrderOrdinal),
            ReorderLevel = reader.IsDBNull(reorderLevelOrdinal) ? null : reader.GetInt16(reorderLevelOrdinal),
            Discontinued = reader.GetBoolean(reader.GetOrdinal("Discontinued"))
        };
    }

    protected static void AddProductParameters(SqlCommand command, Product product)
    {
        command.Parameters.AddWithValue("@ProductName", product.ProductName);
        command.Parameters.AddWithValue("@SupplierID", (object?)product.SupplierId ?? DBNull.Value);
        command.Parameters.AddWithValue("@CategoryID", (object?)product.CategoryId ?? DBNull.Value);
        command.Parameters.AddWithValue("@QuantityPerUnit", (object?)product.QuantityPerUnit ?? DBNull.Value);
        command.Parameters.AddWithValue("@UnitPrice", (object?)product.UnitPrice ?? DBNull.Value);
        command.Parameters.AddWithValue("@UnitsInStock", (object?)product.UnitsInStock ?? DBNull.Value);
        command.Parameters.AddWithValue("@UnitsOnOrder", (object?)product.UnitsOnOrder ?? DBNull.Value);
        command.Parameters.AddWithValue("@ReorderLevel", (object?)product.ReorderLevel ?? DBNull.Value);
        command.Parameters.AddWithValue("@Discontinued", product.Discontinued);
    }
}
