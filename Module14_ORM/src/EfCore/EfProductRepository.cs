using Microsoft.EntityFrameworkCore;
using Module14_ORM.Domain;

namespace Module14_ORM.EfCore;

public sealed class EfProductRepository(OrmDbContext dbContext) : IProductRepository
{
    public async Task<int> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);
        return product.Id;
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Products.AsNoTracking().FirstOrDefaultAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Products.AsNoTracking().OrderBy(product => product.Id).ToListAsync(cancellationToken);
    }

    public async Task<int> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        dbContext.Products.Update(product);
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Product? product = await dbContext.Products.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (product is null)
        {
            return 0;
        }

        dbContext.Products.Remove(product);
        return await dbContext.SaveChangesAsync(cancellationToken);
    }
}