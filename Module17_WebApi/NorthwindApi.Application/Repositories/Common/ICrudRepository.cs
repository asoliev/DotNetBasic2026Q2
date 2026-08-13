namespace NorthwindApi.Repositories.Common;

public interface ICrudRepository<TEntity>
{
    IReadOnlyList<TEntity> GetAll();

    TEntity? GetById(int id);

    TEntity Create(TEntity entity);

    bool Update(TEntity entity);

    bool Delete(int id);
}