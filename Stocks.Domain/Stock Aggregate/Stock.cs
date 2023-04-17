using Stocks.Domain.Exceptions;

namespace Stocks.Domain.Stock_Aggregate;

public class Stock
{
    public Stock(long productId)
    {
        ProductId = productId;
        Id = DateTime.UtcNow.Ticks;
        CreatedAt = DateTime.UtcNow;
    }

    public long ProductId { get; }

    public long Quantity { get; private set; }

    public long Id { get; }

    public DateTime CreatedAt { get; }

    public void IncreaseStock(long quantity)
    {
        if (quantity < 0)
        {
            throw new QuantityMustBePositiveException(quantity);
        }

        Quantity += quantity;
    }

    public void DecreaseStock(long quantity)
    {

        if (quantity < 0)
        {
            throw new QuantityMustBePositiveException(quantity);
        }

        if (quantity > Quantity)
        {
            throw new InsufficientQuantityToDecreaseStockException(ProductId, quantity);
        }

        Quantity -= quantity;
    }
}