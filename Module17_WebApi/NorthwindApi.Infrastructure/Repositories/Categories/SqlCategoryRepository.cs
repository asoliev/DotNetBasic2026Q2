using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using NorthwindApi.Models;
using NorthwindApi.Repositories.Common;

namespace NorthwindApi.Repositories.Categories;

public sealed class SqlCategoryRepository(IConfiguration configuration)
    : SqlCrudRepositoryBase<Category>(configuration), ICategoryRepository
{
    public override IReadOnlyList<Category> GetAll() => GetCategories();

    public override Category? GetById(int id) => GetCategory(id);

    public override Category Create(Category entity) => CreateCategory(entity);

    public override bool Update(Category entity) => UpdateCategory(entity);

    public override bool Delete(int id) => DeleteCategory(id);

    public IReadOnlyList<Category> GetCategories()
    {
        const string sql = """
                           SELECT CategoryID, CategoryName, Description
                           FROM dbo.Categories
                           ORDER BY CategoryID;
                           """;

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        List<Category> categories = [];
        while (reader.Read())
        {
            categories.Add(MapCategory(reader));
        }

        return categories;
    }

    public Category? GetCategory(int categoryId)
    {
        const string sql = """
                           SELECT CategoryID, CategoryName, Description
                           FROM dbo.Categories
                           WHERE CategoryID = @CategoryId;
                           """;

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@CategoryId", categoryId);
        connection.Open();
        using SqlDataReader reader = command.ExecuteReader();

        return reader.Read() ? MapCategory(reader) : null;
    }

    public bool CategoryExists(int categoryId)
    {
        const string sql = "SELECT 1 FROM dbo.Categories WHERE CategoryID = @CategoryId;";
        return Exists(sql, "@CategoryId", categoryId);
    }

    public bool CategoryHasProducts(int categoryId)
    {
        const string sql = "SELECT 1 FROM dbo.Products WHERE CategoryID = @CategoryId;";
        return Exists(sql, "@CategoryId", categoryId);
    }

    public Category CreateCategory(Category category)
    {
        const string sql = """
                           INSERT INTO dbo.Categories (CategoryName, Description)
                           OUTPUT INSERTED.CategoryID
                           VALUES (@CategoryName, @Description);
                           """;

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
        command.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);
        connection.Open();

        int insertedId = Convert.ToInt32(command.ExecuteScalar());
        return GetCategory(insertedId) ?? throw new InvalidOperationException("The inserted category could not be loaded.");
    }

    public bool UpdateCategory(Category category)
    {
        const string sql = """
                           UPDATE dbo.Categories
                           SET CategoryName = @CategoryName,
                               Description = @Description
                           WHERE CategoryID = @CategoryId;
                           """;

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@CategoryId", category.CategoryId);
        command.Parameters.AddWithValue("@CategoryName", category.CategoryName);
        command.Parameters.AddWithValue("@Description", (object?)category.Description ?? DBNull.Value);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }

    public bool DeleteCategory(int categoryId)
    {
        const string sql = "DELETE FROM dbo.Categories WHERE CategoryID = @CategoryId;";

        using SqlConnection connection = CreateConnection();
        using SqlCommand command = new(sql, connection);
        command.Parameters.AddWithValue("@CategoryId", categoryId);
        connection.Open();

        return command.ExecuteNonQuery() > 0;
    }
}