using NorthwindApi.Models;
using NorthwindApi.Repositories.Common;
using NorthwindApi.Services.Common;

namespace NorthwindApi.Repositories.Products;

public interface IProductRepository : ICrudRepository<Product>
{
    PagedResult<Product> GetProducts(int pageNumber = 1, int pageSize = 10, int? categoryId = null);

    Product? GetProduct(int productId);

    bool ProductExists(int productId);

    bool SupplierExists(int supplierId);

    Product CreateProduct(Product product);

    bool UpdateProduct(Product product);

    bool DeleteProduct(int productId);
}