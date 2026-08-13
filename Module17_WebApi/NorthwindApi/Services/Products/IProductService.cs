using NorthwindApi.Models;
using NorthwindApi.Services.Common;

namespace NorthwindApi.Services.Products;

public interface IProductService
{
    IReadOnlyList<Product> GetAll();

    ServiceResult<Product> GetById(int id);

    ServiceResult<Product> Create(ProductUpsertRequest request);

    ServiceResult<bool> Update(int id, ProductUpsertRequest request);

    ServiceResult<bool> Delete(int id);
}