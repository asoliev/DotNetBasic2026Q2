using AdoNetLibrary.Models;

namespace AdoNetLibrary.Repositories;

public interface IProductRepository
{
    int Create(Product product);

    Product? GetById(int id);

    IReadOnlyCollection<Product> GetAll();

    bool Update(Product product);

    bool Delete(int id);
}
