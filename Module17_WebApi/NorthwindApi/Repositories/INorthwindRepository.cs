using NorthwindApi.Models;

namespace NorthwindApi.Repositories;

public interface INorthwindRepository
{
    IReadOnlyList<Category> GetCategories();

    Category? GetCategory(int categoryId);

    Category CreateCategory(Category category);

    bool UpdateCategory(Category category);

    bool DeleteCategory(int categoryId);

    IReadOnlyList<Product> GetProducts();

    Product? GetProduct(int productId);

    Product CreateProduct(Product product);

    bool UpdateProduct(Product product);

    bool DeleteProduct(int productId);
}