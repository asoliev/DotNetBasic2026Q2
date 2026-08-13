using Microsoft.Extensions.Configuration;

namespace NorthwindApi.Repositories.Common;

public abstract class SqlCrudRepositoryBase<TEntity>(IConfiguration configuration)
    : SqlNorthwindRepositoryBase(configuration), ICrudRepository<TEntity>
{
    public abstract IReadOnlyList<TEntity> GetAll();

    public abstract TEntity? GetById(int id);

    public abstract TEntity Create(TEntity entity);

    public abstract bool Update(TEntity entity);

    public abstract bool Delete(int id);
}