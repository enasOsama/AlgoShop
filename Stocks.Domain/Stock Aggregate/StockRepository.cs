using MongoDB.Driver;

namespace Stocks.Domain.Stock_Aggregate;

internal class StockRepository : IStockRepository
{
    private readonly IMongoCollection<Stock> _orders;

    public StockRepository(IMongoDatabase database)
    {
        _orders = database.GetCollection<Stock>("stocks");
    }

    public async Task<Stock?> GetAsync(long id)
    {
        var order = await _orders.FindAsync(x => x.Id == id);
        return await order.FirstOrDefaultAsync();
    }

    public async Task<Stock?> GetByProductIdOrDefaultAsync(long productId)
    {
        var order = await _orders.FindAsync(x => x.ProductId == productId);
        return await order.FirstOrDefaultAsync();
    }

    public async Task AddAsync(Stock stock)
    {
        await _orders.InsertOneAsync(stock);
    }

    public async Task UpdateAsync(Stock stock)
    {
        await _orders.ReplaceOneAsync(x => x.Id == stock.Id, stock);
    }
}