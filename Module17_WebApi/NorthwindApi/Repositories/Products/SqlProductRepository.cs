using Microsoft.Data.SqlClient;
using NorthwindApi.Models;
using NorthwindApi.Repositories.Common;

namespace NorthwindApi.Repositories.Products;

public sealed class SqlProductRepository(IConfiguration configuration)
    : SqlCrudRepositoryBase<Product>(configuration), IProductRepository
{
    public override IReadOnlyList<Product> GetAll() => GetProducts();

    public override Product? GetById(int id) => GetProduct(id);

    public override Product Create(Product entity) => CreateProduct(entity);

    public override bool Update(Product entity) => UpdateProduct(entity);

    public override bool Delete(int id) => DeleteProduct(id);

    public IReadOnlyList<Product> GetProducts()
    {
        const string sql = """
                           SELECT ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit,
                                  UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued
                           FROM dbo.Products
                           ORDER BY ProductID;
                           """;

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        List<Product> products = [];
        while (reader.Read())
        {
            products.Add(MapProduct(reader));
        }

        return products;
    }

    public Product? GetProduct(int productId)
    {
        const string sql = """
                           SELECT ProductID, ProductName, SupplierID, CategoryID, QuantityPerUnit,
                                  UnitPrice, UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued
                           FROM dbo.Products
                           WHERE ProductID = @ProductId;
                           """;

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@ProductId", productId);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapProduct(reader) : null;
    }

    public bool ProductExists(int productId)
    {
        const string sql = "SELECT 1 FROM dbo.Products WHERE ProductID = @ProductId;";
        return Exists(sql, "@ProductId", productId);
    }

    public bool SupplierExists(int supplierId)
    {
        const string sql = "SELECT 1 FROM dbo.Suppliers WHERE SupplierID = @SupplierId;";
        return Exists(sql, "@SupplierId", supplierId);
    }

    public Product CreateProduct(Product product)
    {
        const string sql = """
                           INSERT INTO dbo.Products
                               (ProductName, SupplierID, CategoryID, QuantityPerUnit, UnitPrice,
                                UnitsInStock, UnitsOnOrder, ReorderLevel, Discontinued)
                           OUTPUT INSERTED.ProductID
                           VALUES
                               (@ProductName, @SupplierID, @CategoryID, @QuantityPerUnit, @UnitPrice,
                                @UnitsInStock, @UnitsOnOrder, @ReorderLevel, @Discontinued);
                           """;

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        AddProductParameters(command, product);
        connection.Open();

        int insertedId = Convert.ToInt32(command.ExecuteScalar());
        return GetProduct(insertedId) ?? throw new InvalidOperationException("The inserted product could not be loaded.");
    }

    public bool UpdateProduct(Product product)
    {
        const string sql = """
                           UPDATE dbo.Products
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

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@ProductID", product.ProductId);
        AddProductParameters(command, product);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteProduct(int productId)
    {
        const string sql = "DELETE FROM dbo.Products WHERE ProductID = @ProductId;";

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@ProductId", productId);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }
}