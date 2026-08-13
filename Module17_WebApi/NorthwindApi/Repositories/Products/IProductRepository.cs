using NorthwindApi.Models;
using NorthwindApi.Repositories.Common;

namespace NorthwindApi.Repositories.Products;

public interface IProductRepository : ICrudRepository<Product>
{
    IReadOnlyList<Product> GetProducts();

    Product? GetProduct(int productId);

    bool ProductExists(int productId);

    bool SupplierExists(int supplierId);

    Product CreateProduct(Product product);

    bool UpdateProduct(Product product);

    bool DeleteProduct(int productId);
}