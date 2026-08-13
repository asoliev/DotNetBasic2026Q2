using NorthwindApi.Models;
using NorthwindApi.Services.Common;

namespace NorthwindApi.Services.Products;

public interface IProductService
{
    PagedResult<Product> GetAll(int pageNumber = 1, int pageSize = 10, int? categoryId = null);

    ServiceResult<Product> GetById(int id);

    ServiceResult<Product> Create(ProductUpsertRequest request);

    ServiceResult<bool> Update(int id, ProductUpsertRequest request);

    ServiceResult<bool> Delete(int id);
}