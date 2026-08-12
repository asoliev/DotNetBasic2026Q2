using AdoNetLibrary.Models;

namespace AdoNetLibrary.Repositories;

public interface IOrderRepository
{
    int Create(Order order);

    Order? GetById(int id);

    bool Update(Order order);

    bool Delete(int id);

    IReadOnlyCollection<Order> GetOrders(OrderFilter filter);

    int DeleteOrders(OrderFilter filter);
}
