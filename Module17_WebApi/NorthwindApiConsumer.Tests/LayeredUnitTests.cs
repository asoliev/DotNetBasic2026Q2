using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindApi.Contracts;
using NorthwindApi.Models;
using NorthwindApi.Repositories.Categories;
using NorthwindApi.Repositories.Products;
using NorthwindApi.Services.Categories;
using NorthwindApi.Services.Common;
using NorthwindApi.Services.Products;

namespace NorthwindApiConsumer.Tests;

[TestClass]
public sealed class LayeredUnitTests
{
    [TestMethod]
    public void PagedResult_ComputesTotalPages()
    {
        PagedResult<int> result = new([1, 2, 3], 21, 1, 10);

        Assert.AreEqual(3, result.TotalPages);
    }

    [TestMethod]
    public void ServiceResult_FactoryMethodsSetExpectedStatus()
    {
        Assert.AreEqual(ServiceResultStatus.Success, ServiceResult<int>.Success(1).Status);
        Assert.AreEqual(ServiceResultStatus.NotFound, ServiceResult<int>.NotFound().Status);
        Assert.AreEqual(ServiceResultStatus.Conflict, ServiceResult<int>.Conflict("x").Status);
        Assert.AreEqual(ServiceResultStatus.ValidationFailed, ServiceResult<int>.ValidationFailed(new Dictionary<string, string[]>()).Status);
    }

    [TestMethod]
    public void ToModelStateDictionary_MapsAllErrors()
    {
        IReadOnlyDictionary<string, string[]> errors = new Dictionary<string, string[]>
        {
            ["CategoryId"] = ["Missing"],
            ["SupplierId"] = ["Invalid", "Unknown"]
        };

        ModelStateDictionary modelState = errors.ToModelStateDictionary();

        Assert.AreEqual(2, modelState.Count);
        Assert.IsNotNull(modelState["CategoryId"]);
        Assert.IsNotNull(modelState["SupplierId"]);
        Assert.AreEqual(1, modelState["CategoryId"]!.Errors.Count);
        Assert.AreEqual(2, modelState["SupplierId"]!.Errors.Count);
    }

    [TestMethod]
    public void CategoryService_CoversSuccessAndFailurePaths()
    {
        FakeCategoryRepository repository = new()
        {
            Categories =
            [
                new Category { CategoryId = 1, CategoryName = "Seafood" }
            ],
            CategoryHasProductsResult = true
        };

        CategoryService service = new(repository);

        Assert.AreEqual(1, service.GetAll().Count);
        Assert.AreEqual(ServiceResultStatus.Success, service.GetById(1).Status);
        Assert.AreEqual(ServiceResultStatus.NotFound, service.GetById(2).Status);
        Assert.AreEqual(ServiceResultStatus.Success, service.Create(new CategoryUpsertRequest { CategoryName = "Drinks" }).Status);
        Assert.AreEqual(ServiceResultStatus.Success, service.Update(1, new CategoryUpsertRequest { CategoryName = "Seafood Updated" }).Status);
        Assert.AreEqual(ServiceResultStatus.Conflict, service.Delete(1).Status);

        repository.CategoryHasProductsResult = false;
        Assert.AreEqual(ServiceResultStatus.Success, service.Delete(1).Status);
    }

    [TestMethod]
    public void ProductService_CoversValidationAndMutationPaths()
    {
        FakeCategoryRepository categoryRepository = new()
        {
            Categories =
            [
                new Category { CategoryId = 1, CategoryName = "Seafood" }
            ]
        };

        FakeProductRepository productRepository = new()
        {
            Products =
            [
                new Product { ProductId = 1, ProductName = "Chai", CategoryId = 1, SupplierId = 1 }
            ],
            SupplierIds = [1]
        };

        ProductService service = new(productRepository, categoryRepository);

        Assert.AreEqual(10, service.GetAll(-1, -1).PageSize);
        Assert.AreEqual(ServiceResultStatus.Success, service.GetById(1).Status);
        Assert.AreEqual(ServiceResultStatus.NotFound, service.GetById(2).Status);

        ServiceResult<Product> validationFailed = service.Create(new ProductUpsertRequest
        {
            ProductName = "Broken",
            CategoryId = 99,
            SupplierId = 99
        });

        Assert.AreEqual(ServiceResultStatus.ValidationFailed, validationFailed.Status);

        ServiceResult<Product> created = service.Create(new ProductUpsertRequest
        {
            ProductName = "Coffee",
            CategoryId = 1,
            SupplierId = 1,
            Discontinued = false
        });

        Assert.AreEqual(ServiceResultStatus.Success, created.Status);
        Assert.IsTrue(created.Value is not null);

        Assert.AreEqual(ServiceResultStatus.NotFound, service.Update(999, new ProductUpsertRequest
        {
            ProductName = "Coffee Updated",
            CategoryId = 1,
            SupplierId = 1
        }).Status);

        Assert.AreEqual(ServiceResultStatus.Success, service.Update(1, new ProductUpsertRequest
        {
            ProductName = "Chai Updated",
            CategoryId = 1,
            SupplierId = 1
        }).Status);

        Assert.AreEqual(ServiceResultStatus.Success, service.Delete(1).Status);
    }

    private sealed class FakeCategoryRepository : ICategoryRepository
    {
        public List<Category> Categories { get; set; } = [];

        public bool CategoryHasProductsResult { get; set; }

        public IReadOnlyList<Category> GetAll() => GetCategories();

        public Category? GetById(int id) => GetCategory(id);

        public Category Create(Category entity) => CreateCategory(entity);

        public bool Update(Category entity) => UpdateCategory(entity);

        public bool Delete(int id) => DeleteCategory(id);

        public IReadOnlyList<Category> GetCategories() => Categories;

        public Category? GetCategory(int categoryId) => Categories.FirstOrDefault(category => category.CategoryId == categoryId);

        public bool CategoryExists(int categoryId) => Categories.Any(category => category.CategoryId == categoryId);

        public bool CategoryHasProducts(int categoryId) => CategoryHasProductsResult;

        public Category CreateCategory(Category category)
        {
            category.CategoryId = Categories.Count == 0 ? 1 : Categories.Max(item => item.CategoryId) + 1;
            Categories.Add(category);
            return category;
        }

        public bool UpdateCategory(Category category)
        {
            int index = Categories.FindIndex(item => item.CategoryId == category.CategoryId);
            if (index < 0)
                return false;

            Categories[index] = category;
            return true;
        }

        public bool DeleteCategory(int categoryId)
        {
            Category? category = GetCategory(categoryId);
            if (category is null)
                return false;

            Categories.Remove(category);
            return true;
        }
    }

    private sealed class FakeProductRepository : IProductRepository
    {
        public List<Product> Products { get; set; } = [];

        public HashSet<int> SupplierIds { get; set; } = [];

        public IReadOnlyList<Product> GetAll() => GetProducts().Items;

        public Product? GetById(int id) => GetProduct(id);

        public Product Create(Product entity) => CreateProduct(entity);

        public bool Update(Product entity) => UpdateProduct(entity);

        public bool Delete(int id) => DeleteProduct(id);

        public PagedResult<Product> GetProducts(int pageNumber = 1, int pageSize = 10, int? categoryId = null)
        {
            IEnumerable<Product> query = Products;
            if (categoryId is not null)
                query = query.Where(product => product.CategoryId == categoryId);

            List<Product> filtered = query.ToList();
            List<Product> page = filtered
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<Product>(page, filtered.Count, pageNumber, pageSize);
        }

        public Product? GetProduct(int productId) => Products.FirstOrDefault(product => product.ProductId == productId);

        public bool ProductExists(int productId) => Products.Any(product => product.ProductId == productId);

        public bool SupplierExists(int supplierId) => SupplierIds.Contains(supplierId);

        public Product CreateProduct(Product product)
        {
            product.ProductId = Products.Count == 0 ? 1 : Products.Max(item => item.ProductId) + 1;
            Products.Add(product);
            return product;
        }

        public bool UpdateProduct(Product product)
        {
            int index = Products.FindIndex(item => item.ProductId == product.ProductId);
            if (index < 0)
                return false;

            Products[index] = product;
            return true;
        }

        public bool DeleteProduct(int productId)
        {
            Product? product = GetProduct(productId);
            if (product is null)
                return false;

            Products.Remove(product);
            return true;
        }
    }
}
