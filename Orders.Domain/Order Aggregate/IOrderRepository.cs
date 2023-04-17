namespace Orders.Domain.Order_Aggregate;

public interface IOrderRepository
{
    Task<Order> GetAsync(long id);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}