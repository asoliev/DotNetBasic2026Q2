using NorthwindApi.Models;
using NorthwindApi.Repositories.Categories;
using NorthwindApi.Repositories.Products;
using NorthwindApi.Services.Common;

namespace NorthwindApi.Services.Products;

public sealed class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository) : IProductService
{
    public PagedResult<Product> GetAll(int pageNumber = 1, int pageSize = 10, int? categoryId = null)
    {
        int normalizedPageNumber = pageNumber < 1 ? 1 : pageNumber;
        int normalizedPageSize = pageSize < 1 ? 10 : pageSize;

        return productRepository.GetProducts(normalizedPageNumber, normalizedPageSize, categoryId);
    }

    public ServiceResult<Product> GetById(int id)
    {
        Product? product = productRepository.GetProduct(id);
        return product is null ? ServiceResult<Product>.NotFound() : ServiceResult<Product>.Success(product);
    }

    public ServiceResult<Product> Create(ProductUpsertRequest request)
    {
        ServiceResult<bool>? validationResult = ValidateRequest(request);
        if (validationResult is not null)
            return ServiceResult<Product>.ValidationFailed(validationResult.Errors!);

        Product created = productRepository.CreateProduct(MapRequest(request));
        return ServiceResult<Product>.Success(created);
    }

    public ServiceResult<bool> Update(int id, ProductUpsertRequest request)
    {
        if (!productRepository.ProductExists(id))
            return ServiceResult<bool>.NotFound();

        ServiceResult<bool>? validationResult = ValidateRequest(request);
        if (validationResult is not null)
            return validationResult;

        bool updated = productRepository.UpdateProduct(MapRequest(request, id));
        return updated ? ServiceResult<bool>.Success(true) : ServiceResult<bool>.NotFound();
    }

    public ServiceResult<bool> Delete(int id)
    {
        bool deleted = productRepository.DeleteProduct(id);
        return deleted ? ServiceResult<bool>.Success(true) : ServiceResult<bool>.NotFound();
    }

    private ServiceResult<bool>? ValidateRequest(ProductUpsertRequest request)
    {
        Dictionary<string, string[]> errors = [];

        if (request.CategoryId is not null && !categoryRepository.CategoryExists(request.CategoryId.Value))
            errors[nameof(request.CategoryId)] = ["The specified category does not exist."];

        if (request.SupplierId is not null && !productRepository.SupplierExists(request.SupplierId.Value))
            errors[nameof(request.SupplierId)] = ["The specified supplier does not exist."];

        return errors.Count == 0 ? null : ServiceResult<bool>.ValidationFailed(errors);
    }

    private static Product MapRequest(ProductUpsertRequest request, int productId = 0) => new()
    {
        ProductId = productId,
        ProductName = request.ProductName,
        SupplierId = request.SupplierId,
        CategoryId = request.CategoryId,
        QuantityPerUnit = request.QuantityPerUnit,
        UnitPrice = request.UnitPrice,
        UnitsInStock = request.UnitsInStock,
        UnitsOnOrder = request.UnitsOnOrder,
        ReorderLevel = request.ReorderLevel,
        Discontinued = request.Discontinued
    };
}