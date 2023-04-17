namespace Stocks.Domain.Stock_Aggregate;

public interface IStockRepository
{
    Task<Stock?> GetAsync(long id);

    Task AddAsync(Stock stock);

    Task UpdateAsync(Stock stock);

    Task<Stock?> GetByProductIdOrDefaultAsync(long productId);
}