using System.Data;
using AdoNetLibrary.Models;
using Microsoft.Data.SqlClient;

namespace AdoNetLibrary.Repositories;

public sealed class ProductRepository(string connectionString) : IProductRepository
{
    private readonly SqlConnectionFactory connectionFactory = new(connectionString);

    public int Create(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            INSERT INTO dbo.Products (Name, Price)
            OUTPUT INSERTED.Id
            VALUES (@name, @price);
            """,
            connection
        );

        command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@price", product.Price);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public Product? GetById(int id)
    {
        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            SELECT Id, Name, Price
            FROM dbo.Products
            WHERE Id = @id;
            """,
            connection
        );
        command.Parameters.AddWithValue("@id", id);

        using SqlDataReader reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return new()
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Price = reader.GetDecimal(2),
        };
    }

    public IReadOnlyCollection<Product> GetAll()
    {
        // Disconnected model usage with DataTable + DataAdapter.
        var dataTable = new DataTable();

        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            SELECT Id, Name, Price
            FROM dbo.Products
            ORDER BY Id;
            """,
            connection
        );

        using SqlDataAdapter adapter = new(command);
        adapter.Fill(dataTable);

        List<Product> products = new(dataTable.Rows.Count);
        foreach (DataRow row in dataTable.Rows)
        {
            products.Add(new()
            {
                Id = Convert.ToInt32(row["Id"]),
                Name = Convert.ToString(row["Name"]) ?? string.Empty,
                Price = Convert.ToDecimal(row["Price"]),
            });
        }

        return products;
    }

    public bool Update(Product product)
    {
        ArgumentNullException.ThrowIfNull(product);

        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            UPDATE dbo.Products
            SET Name = @name,
                Price = @price
            WHERE Id = @id;
            """,
            connection
        );

        command.Parameters.AddWithValue("@id", product.Id);
        command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@price", product.Price);

        return command.ExecuteNonQuery() > 0;
    }

    public bool Delete(int id)
    {
        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            DELETE FROM dbo.Products
            WHERE Id = @id;
            """,
            connection
        );
        command.Parameters.AddWithValue("@id", id);

        return command.ExecuteNonQuery() > 0;
    }
}
