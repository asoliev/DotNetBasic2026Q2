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
            INSERT INTO dbo.Products (Name, Description, Weight, Height, Width, Length)
            OUTPUT INSERTED.Id
            VALUES (@name, @description, @weight, @height, @width, @length);
            """,
            connection
        );

        command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@description", product.Description);
        command.Parameters.AddWithValue("@weight", product.Weight);
        command.Parameters.AddWithValue("@height", product.Height);
        command.Parameters.AddWithValue("@width", product.Width);
        command.Parameters.AddWithValue("@length", product.Length);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public Product? GetById(int id)
    {
        using SqlConnection connection = this.connectionFactory.CreateOpenConnection();
        using SqlCommand command = new
        (
            """
            SELECT Id, Name, Description, Weight, Height, Width, Length
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
            Description = reader.GetString(2),
            Weight = reader.GetDecimal(3),
            Height = reader.GetDecimal(4),
            Width = reader.GetDecimal(5),
            Length = reader.GetDecimal(6),
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
            SELECT Id, Name, Description, Weight, Height, Width, Length
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
                Description = Convert.ToString(row["Description"]) ?? string.Empty,
                Weight = Convert.ToDecimal(row["Weight"]),
                Height = Convert.ToDecimal(row["Height"]),
                Width = Convert.ToDecimal(row["Width"]),
                Length = Convert.ToDecimal(row["Length"]),
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
                Description = @description,
                Weight = @weight,
                Height = @height,
                Width = @width,
                Length = @length
            WHERE Id = @id;
            """,
            connection
        );

        command.Parameters.AddWithValue("@id", product.Id);
        command.Parameters.AddWithValue("@name", product.Name);
        command.Parameters.AddWithValue("@description", product.Description);
        command.Parameters.AddWithValue("@weight", product.Weight);
        command.Parameters.AddWithValue("@height", product.Height);
        command.Parameters.AddWithValue("@width", product.Width);
        command.Parameters.AddWithValue("@length", product.Length);

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
