using Microsoft.Data.SqlClient;
using NorthwindMvc.Models;

namespace NorthwindMvc.Data;

public sealed class NorthwindRepository(IConfiguration configuration)
{
    private readonly string connectionString = configuration.GetConnectionString("Northwind")
            ?? throw new InvalidOperationException("Connection string 'Northwind' was not found.");

    public async Task<IReadOnlyList<CategoryListItem>> GetCategoriesAsync()
    {
        List<CategoryListItem> categories = [];

        await using SqlConnection connection = new(connectionString);
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

    public async Task<CategoryDetailsViewModel?> GetCategoryDetailsAsync(int categoryId)
    {
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT CategoryID, CategoryName, Description, CAST(Picture AS varbinary(max))
            FROM Categories
            WHERE CategoryID = @CategoryID;
            """;
        command.Parameters.AddWithValue("@CategoryID", categoryId);

        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        byte[]? picture = reader.IsDBNull(3) ? null : (byte[])reader.GetValue(3);

        return new CategoryDetailsViewModel
        {
            CategoryID = reader.GetInt32(0),
            CategoryName = reader.GetString(1),
            Description = reader.IsDBNull(2) ? null : reader.GetString(2),
            PictureDataUrl = CreatePictureDataUrl(picture)
        };
    }

    public async Task<IReadOnlyList<ProductListItem>> GetProductsAsync(int maximumProducts)
    {
        List<ProductListItem> products = [];
        string topClause = maximumProducts > 0 ? "TOP (@MaximumProducts)" : string.Empty;

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = $"""
            SELECT {topClause}
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

        if (maximumProducts > 0)
            command.Parameters.AddWithValue("@MaximumProducts", maximumProducts);

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

    public async Task<ProductDetailsViewModel?> GetProductDetailsAsync(int productId)
    {
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT P.ProductID,
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
            WHERE P.ProductID = @ProductID;
            """;
        command.Parameters.AddWithValue("@ProductID", productId);

        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new ProductDetailsViewModel
        {
            ProductID = reader.GetInt32(0),
            ProductName = reader.GetString(1),
            SupplierName = reader.GetString(2),
            CategoryName = reader.IsDBNull(3) ? null : reader.GetString(3),
            QuantityPerUnit = reader.IsDBNull(4) ? null : reader.GetString(4),
            UnitPrice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
            UnitsInStock = reader.IsDBNull(6) ? null : reader.GetInt16(6),
            UnitsOnOrder = reader.IsDBNull(7) ? null : reader.GetInt16(7),
            ReorderLevel = reader.IsDBNull(8) ? null : reader.GetInt16(8),
            Discontinued = reader.GetBoolean(9)
        };
    }

    public async Task<IReadOnlyList<SupplierListItem>> GetSuppliersAsync()
    {
        List<SupplierListItem> suppliers = [];

        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT SupplierID, CompanyName
            FROM Suppliers
            ORDER BY CompanyName;
            """;

        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            suppliers.Add(new SupplierListItem(
                reader.GetInt32(0),
                reader.GetString(1)));
        }

        return suppliers;
    }

    public async Task<ProductEditViewModel?> GetProductForEditAsync(int productId)
    {
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = """
            SELECT ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit,
                   UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued
            FROM Products
            WHERE ProductID = @ProductID;
            """;
        command.Parameters.AddWithValue("@ProductID", productId);

        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return null;

        return new ProductEditViewModel
        {
            ProductID = reader.GetInt32(0),
            ProductName = reader.GetString(1),
            SupplierID = reader.IsDBNull(2) ? null : reader.GetInt32(2),
            CategoryID = reader.IsDBNull(3) ? null : reader.GetInt32(3),
            QuantityPerUnit = reader.IsDBNull(4) ? null : reader.GetString(4),
            UnitPrice = reader.IsDBNull(5) ? null : reader.GetDecimal(5),
            UnitsInStock = reader.IsDBNull(6) ? null : reader.GetInt16(6),
            UnitsOnOrder = reader.IsDBNull(7) ? null : reader.GetInt16(7),
            ReorderLevel = reader.IsDBNull(8) ? null : reader.GetInt16(8),
            Discontinued = reader.GetBoolean(9)
        };
    }

    public async Task CreateProductAsync(ProductEditViewModel model)
    {
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Products
                (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice,
                 UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued)
            VALUES
                (@ProductName, @SupplierID, @CategoryID, @QuantityPerUnit, @UnitPrice,
                 @UnitsInStock, @UnitsOnOrder, @ReorderLevel, @Discontinued);
            """;

        AddProductParameters(command, model);
        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> UpdateProductAsync(ProductEditViewModel model)
    {
        await using SqlConnection connection = new(connectionString);
        await connection.OpenAsync();

        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Products
            SET ProductName = @ProductName,
                SupplierID = @SupplierID,
                CategoryID = @CategoryID,
                QuantityPerUnit = @QuantityPerUnit,
                UnitPrice = @UnitPrice,
                UnitsInStock = @UnitsInStock,
                UnitsOnOrder = @UnitsOnOrder,
                ReorderLevel = @ReorderLevel,
                Discontinued = @Discontinued
            WHERE ProductID = @ProductID;
            """;

        AddProductParameters(command, model);
        command.Parameters.AddWithValue("@ProductID", model.ProductID);

        int rowsAffected = await command.ExecuteNonQueryAsync();
        return rowsAffected > 0;
    }

    private static void AddProductParameters(SqlCommand command, ProductEditViewModel model)
    {
        command.Parameters.AddWithValue("@ProductName", model.ProductName);
        command.Parameters.AddWithValue("@SupplierID", model.SupplierID ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@CategoryID", model.CategoryID ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@QuantityPerUnit", (object?)model.QuantityPerUnit ?? DBNull.Value);
        command.Parameters.AddWithValue("@UnitPrice", model.UnitPrice ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@UnitsInStock", model.UnitsInStock ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@UnitsOnOrder", model.UnitsOnOrder ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@ReorderLevel", model.ReorderLevel ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Discontinued", model.Discontinued);
    }

    private static string? CreatePictureDataUrl(byte[]? picture)
    {
        if (picture is null || picture.Length == 0)
            return null;

        byte[] imageBytes = StripOleHeader(picture);
        string mimeType = DetectMimeType(imageBytes);
        return $"data:{mimeType};base64,{Convert.ToBase64String(imageBytes)}";
    }

    private static byte[] StripOleHeader(byte[] picture)
    {
        int offset = FindImageOffset(picture);
        if (offset <= 0)
            return picture;

        byte[] imageBytes = new byte[picture.Length - offset];
        Buffer.BlockCopy(picture, offset, imageBytes, 0, imageBytes.Length);
        return imageBytes;
    }

    private static int FindImageOffset(byte[] picture)
    {
        byte[] jpegSignature = [0xFF, 0xD8, 0xFF];
        byte[] pngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        byte[] gifSignature = [0x47, 0x49, 0x46, 0x38];
        byte[] bmpSignature = [0x42, 0x4D];

        return IndexOf(picture, jpegSignature)
            ?? IndexOf(picture, pngSignature)
            ?? IndexOf(picture, gifSignature)
            ?? IndexOf(picture, bmpSignature)
            ?? 0;
    }

    private static int? IndexOf(byte[] source, byte[] pattern)
    {
        for (int index = 0; index <= source.Length - pattern.Length; index++)
        {
            bool match = true;

            for (int patternIndex = 0; patternIndex < pattern.Length; patternIndex++)
            {
                if (source[index + patternIndex] != pattern[patternIndex])
                {
                    match = false;
                    break;
                }
            }

            if (match)
                return index;
        }

        return null;
    }

    private static string DetectMimeType(byte[] imageBytes)
    {
        if (StartsWith(imageBytes, [0xFF, 0xD8, 0xFF]))
            return "image/jpeg";

        if (StartsWith(imageBytes, [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A]))
            return "image/png";

        if (StartsWith(imageBytes, [0x47, 0x49, 0x46, 0x38]))
            return "image/gif";

        if (StartsWith(imageBytes, [0x42, 0x4D]))
            return "image/bmp";

        return "image/jpeg";
    }

    private static bool StartsWith(byte[] source, byte[] pattern)
    {
        if (source.Length < pattern.Length)
            return false;

        for (int index = 0; index < pattern.Length; index++)
        {
            if (source[index] != pattern[index])
                return false;
        }

        return true;
    }
}
