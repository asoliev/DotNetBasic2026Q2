using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindApi.Models;
using NorthwindApi.Repositories.Categories;
using NorthwindApi.Repositories.Products;
using NorthwindApi.Services.Common;

namespace NorthwindApiConsumer.Tests;

[TestClass]
public sealed class RepositoryIntegrationTests
{
    [TestMethod]
    public void SqlCategoryRepository_CanReadAndMutateCategories()
    {
        IConfiguration configuration = CreateConfiguration();
        SqlCategoryRepository categoryRepository = new(configuration);

        IReadOnlyList<Category> categories = categoryRepository.GetCategories();
        Assert.IsTrue(categories.Count > 0);

        Category categoryWithProducts = categories.First(category => categoryRepository.CategoryHasProducts(category.CategoryId));
        Assert.IsNotNull(categoryRepository.GetCategory(categoryWithProducts.CategoryId));
        Assert.IsTrue(categoryRepository.CategoryExists(categoryWithProducts.CategoryId));
        Assert.IsTrue(categoryRepository.CategoryHasProducts(categoryWithProducts.CategoryId));

        string stamp = DateTime.UtcNow.ToString("HHmmssfff");
        Category createdCategory = categoryRepository.CreateCategory(new Category
        {
            CategoryName = $"C{stamp}",
            Description = "Temporary integration category"
        });

        try
        {
            createdCategory.Description = "Updated temporary integration category";
            Assert.IsTrue(categoryRepository.UpdateCategory(createdCategory));
            Assert.AreEqual("Updated temporary integration category", categoryRepository.GetCategory(createdCategory.CategoryId)!.Description);
            Assert.IsTrue(categoryRepository.DeleteCategory(createdCategory.CategoryId));
        }
        finally
        {
            _ = categoryRepository.DeleteCategory(createdCategory.CategoryId);
        }
    }

    [TestMethod]
    public void SqlProductRepository_CanReadAndMutateProducts()
    {
        IConfiguration configuration = CreateConfiguration();
        SqlCategoryRepository categoryRepository = new(configuration);
        SqlProductRepository productRepository = new(configuration);

        PagedResult<Product> firstPage = productRepository.GetProducts(1, 5);
        Assert.IsTrue(firstPage.Items.Count > 0);

        Product productWithSupplier = firstPage.Items.First(product => product.SupplierId is not null);
        Assert.IsNotNull(productRepository.GetProduct(productWithSupplier.ProductId));
        Assert.IsTrue(productRepository.ProductExists(productWithSupplier.ProductId));
        Assert.IsTrue(productRepository.SupplierExists(productWithSupplier.SupplierId!.Value));

        PagedResult<Product> filtered = productRepository.GetProducts(1, 5, productWithSupplier.CategoryId);
        Assert.IsTrue(filtered.Items.All(product => product.CategoryId == productWithSupplier.CategoryId));

        string stamp = DateTime.UtcNow.ToString("HHmmssfff");
        Category createdCategory = categoryRepository.CreateCategory(new Category
        {
            CategoryName = $"P{stamp}",
            Description = "Temporary product integration category"
        });

        Product createdProduct = productRepository.CreateProduct(new Product
        {
            ProductName = $"Product {stamp}",
            SupplierId = 1,
            CategoryId = createdCategory.CategoryId,
            QuantityPerUnit = "1 box",
            UnitPrice = 12.34m,
            UnitsInStock = 1,
            UnitsOnOrder = 0,
            ReorderLevel = 0,
            Discontinued = false
        });

        try
        {
            createdProduct.ProductName = $"Product {stamp} Updated";
            createdProduct.QuantityPerUnit = "2 boxes";
            Assert.IsTrue(productRepository.UpdateProduct(createdProduct));
            Assert.AreEqual(createdProduct.ProductName, productRepository.GetProduct(createdProduct.ProductId)!.ProductName);
            Assert.IsTrue(productRepository.DeleteProduct(createdProduct.ProductId));
        }
        finally
        {
            _ = productRepository.DeleteProduct(createdProduct.ProductId);
            _ = categoryRepository.DeleteCategory(createdCategory.CategoryId);
        }
    }

    private static IConfiguration CreateConfiguration() => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["ConnectionStrings:Northwind"] = "Server=localhost,1433;Database=Northwind;User Id=sa;Password=YourStrongP@ssw0rd!;Encrypt=True;TrustServerCertificate=True"
        })
        .Build();
}