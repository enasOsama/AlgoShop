using MongoDB.Driver;

namespace Orders.Domain.Order_Aggregate;

internal class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders;

    public OrderRepository(IMongoDatabase database)
    {
        _orders = database.GetCollection<Order>("orders");
    }

    public async Task<Order> GetAsync(long id)
    {
        var order = await _orders.FindAsync(x => x.Id == id);
        return await order.FirstOrDefaultAsync();
    }

    public async Task AddAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        await _orders.ReplaceOneAsync(x => x.Id == order.Id, order);
    }
}