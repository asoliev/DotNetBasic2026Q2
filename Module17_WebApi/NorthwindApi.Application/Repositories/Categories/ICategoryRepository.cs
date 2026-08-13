using NorthwindApi.Models;
using NorthwindApi.Repositories.Common;

namespace NorthwindApi.Repositories.Categories;

public interface ICategoryRepository : ICrudRepository<Category>
{
    IReadOnlyList<Category> GetCategories();

    Category? GetCategory(int categoryId);

    bool CategoryExists(int categoryId);

    bool CategoryHasProducts(int categoryId);

    Category CreateCategory(Category category);

    bool UpdateCategory(Category category);

    bool DeleteCategory(int categoryId);
}