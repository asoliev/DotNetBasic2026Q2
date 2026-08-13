using NorthwindApi.Models;

namespace NorthwindApi.Repositories;

public sealed class InMemoryNorthwindRepository : INorthwindRepository
{
    private readonly object syncRoot = new();
    private readonly List<Category> categories = [];
    private readonly List<Product> products = [];
    private int nextCategoryId = 1;
    private int nextProductId = 1;

    public InMemoryNorthwindRepository()
    {
        Category beverages = new()
        {
            CategoryId = GetNextCategoryId(),
            CategoryName = "Beverages",
            Description = "Soft drinks, coffees, teas, beers, and ales"
        };

        Category condiments = new()
        {
            CategoryId = GetNextCategoryId(),
            CategoryName = "Condiments",
            Description = "Sweet and savory sauces, relishes, spreads, and seasonings"
        };

        categories.AddRange([beverages, condiments]);

        products.AddRange(
        [
            new Product
            {
                ProductId = GetNextProductId(),
                ProductName = "Chai",
                SupplierId = 1,
                CategoryId = beverages.CategoryId,
                QuantityPerUnit = "10 boxes x 20 bags",
                UnitPrice = 18.00m,
                UnitsInStock = 39,
                UnitsOnOrder = 0,
                ReorderLevel = 10,
                Discontinued = false
            },
            new Product
            {
                ProductId = GetNextProductId(),
                ProductName = "Aniseed Syrup",
                SupplierId = 1,
                CategoryId = condiments.CategoryId,
                QuantityPerUnit = "12 - 550 ml bottles",
                UnitPrice = 10.00m,
                UnitsInStock = 13,
                UnitsOnOrder = 70,
                ReorderLevel = 25,
                Discontinued = false
            }
        ]);
    }

    public IReadOnlyList<Category> GetCategories()
    {
        lock (syncRoot)
        {
            return categories.Select(CloneCategory).ToList();
        }
    }

    public Category? GetCategory(int categoryId)
    {
        lock (syncRoot)
        {
            Category? category = categories.FirstOrDefault(current => current.CategoryId == categoryId);
            return category is null ? null : CloneCategory(category);
        }
    }

    public Category CreateCategory(Category category)
    {
        lock (syncRoot)
        {
            Category newCategory = CloneCategory(category);
            newCategory.CategoryId = GetNextCategoryId();
            categories.Add(newCategory);
            return CloneCategory(newCategory);
        }
    }

    public bool UpdateCategory(Category category)
    {
        lock (syncRoot)
        {
            int index = categories.FindIndex(current => current.CategoryId == category.CategoryId);
            if (index < 0)
            {
                return false;
            }

            categories[index] = CloneCategory(category);
            return true;
        }
    }

    public bool DeleteCategory(int categoryId)
    {
        lock (syncRoot)
        {
            if (products.Any(product => product.CategoryId == categoryId))
            {
                return false;
            }

            int removed = categories.RemoveAll(category => category.CategoryId == categoryId);
            return removed > 0;
        }
    }

    public IReadOnlyList<Product> GetProducts()
    {
        lock (syncRoot)
        {
            return products.Select(CloneProduct).ToList();
        }
    }

    public Product? GetProduct(int productId)
    {
        lock (syncRoot)
        {
            Product? product = products.FirstOrDefault(current => current.ProductId == productId);
            return product is null ? null : CloneProduct(product);
        }
    }

    public Product CreateProduct(Product product)
    {
        lock (syncRoot)
        {
            Product newProduct = CloneProduct(product);
            newProduct.ProductId = GetNextProductId();
            products.Add(newProduct);
            return CloneProduct(newProduct);
        }
    }

    public bool UpdateProduct(Product product)
    {
        lock (syncRoot)
        {
            int index = products.FindIndex(current => current.ProductId == product.ProductId);
            if (index < 0)
            {
                return false;
            }

            products[index] = CloneProduct(product);
            return true;
        }
    }

    public bool DeleteProduct(int productId)
    {
        lock (syncRoot)
        {
            int removed = products.RemoveAll(product => product.ProductId == productId);
            return removed > 0;
        }
    }

    private int GetNextCategoryId() => nextCategoryId++;

    private int GetNextProductId() => nextProductId++;

    private static Category CloneCategory(Category category) => new()
    {
        CategoryId = category.CategoryId,
        CategoryName = category.CategoryName,
        Description = category.Description
    };

    private static Product CloneProduct(Product product) => new()
    {
        ProductId = product.ProductId,
        ProductName = product.ProductName,
        SupplierId = product.SupplierId,
        CategoryId = product.CategoryId,
        QuantityPerUnit = product.QuantityPerUnit,
        UnitPrice = product.UnitPrice,
        UnitsInStock = product.UnitsInStock,
        UnitsOnOrder = product.UnitsOnOrder,
        ReorderLevel = product.ReorderLevel,
        Discontinued = product.Discontinued
    };
}