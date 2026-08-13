using Microsoft.EntityFrameworkCore;
using Module14_ORM.Domain;

namespace Module14_ORM.EfCore;

public sealed class EfOrderRepository(OrmDbContext dbContext, IOrderStoredProcedureGateway storedProcedureGateway) : IOrderRepository
{
    public async Task<int> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);
        return order.Id;
    }

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(order => order.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders.AsNoTracking().OrderBy(order => order.Id).ToListAsync(cancellationToken);
    }

    public async Task<int> UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        dbContext.Orders.Update(order);
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        Order? order = await dbContext.Orders.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (order is null)
        {
            return 0;
        }

        dbContext.Orders.Remove(order);
        return await dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<IReadOnlyList<Order>> GetFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        return storedProcedureGateway.GetFilteredAsync(filter, cancellationToken);
    }

    public Task<int> DeleteFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        return storedProcedureGateway.DeleteFilteredAsync(filter, cancellationToken);
    }
}

public sealed class EfOrderStoredProcedureGateway(OrmDbContext dbContext) : IOrderStoredProcedureGateway
{
    public async Task<IReadOnlyList<Order>> GetFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        FormattableString sql = $"""
            EXEC dbo.usp_Orders_GetFiltered
                @Month={filter.Month},
                @Status={filter.Status},
                @Year={filter.Year},
                @ProductId={filter.ProductId}
            """;

        return await dbContext.Orders.FromSqlInterpolated(sql).AsNoTracking().ToListAsync(cancellationToken);
    }

    public Task<int> DeleteFilteredAsync(OrderFilter filter, CancellationToken cancellationToken = default)
    {
        FormattableString sql = $"""
            EXEC dbo.usp_Orders_DeleteFiltered
                @Month={filter.Month},
                @Status={filter.Status},
                @Year={filter.Year},
                @ProductId={filter.ProductId}
            """;

        return dbContext.Database.ExecuteSqlInterpolatedAsync(sql, cancellationToken);
    }
}