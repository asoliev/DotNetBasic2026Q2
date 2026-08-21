using NorthwindApi.Contracts;
using NorthwindApi.Models;
using NorthwindApi.Services.Common;

namespace NorthwindApi.Services.Categories;

public interface ICategoryService
{
    IReadOnlyList<Category> GetAll();

    ServiceResult<Category> GetById(int id);

    ServiceResult<Category> Create(CategoryUpsertRequest request);

    ServiceResult<bool> Update(int id, CategoryUpsertRequest request);

    ServiceResult<bool> Delete(int id);
}
