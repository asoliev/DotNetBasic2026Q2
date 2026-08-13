using NorthwindApi.Models;
using NorthwindApi.Repositories.Categories;
using NorthwindApi.Services.Common;

namespace NorthwindApi.Services.Categories;

public sealed class CategoryService(ICategoryRepository repository) : ICategoryService
{
    public IReadOnlyList<Category> GetAll() => repository.GetCategories();

    public ServiceResult<Category> GetById(int id)
    {
        Category? category = repository.GetCategory(id);
        return category is null ? ServiceResult<Category>.NotFound() : ServiceResult<Category>.Success(category);
    }

    public ServiceResult<Category> Create(CategoryUpsertRequest request)
    {
        Category created = repository.CreateCategory(new Category
        {
            CategoryName = request.CategoryName,
            Description = request.Description
        });

        return ServiceResult<Category>.Success(created);
    }

    public ServiceResult<bool> Update(int id, CategoryUpsertRequest request)
    {
        if (!repository.CategoryExists(id))
            return ServiceResult<bool>.NotFound();

        bool updated = repository.UpdateCategory(new Category
        {
            CategoryId = id,
            CategoryName = request.CategoryName,
            Description = request.Description
        });

        return updated ? ServiceResult<bool>.Success(true) : ServiceResult<bool>.NotFound();
    }

    public ServiceResult<bool> Delete(int id)
    {
        if (!repository.CategoryExists(id))
            return ServiceResult<bool>.NotFound();

        if (repository.CategoryHasProducts(id))
            return ServiceResult<bool>.Conflict("Delete the products in the category first.");

        bool deleted = repository.DeleteCategory(id);
        return deleted ? ServiceResult<bool>.Success(true) : ServiceResult<bool>.NotFound();
    }
}