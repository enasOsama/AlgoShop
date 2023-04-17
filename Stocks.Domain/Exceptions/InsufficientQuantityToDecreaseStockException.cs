namespace Stocks.Domain.Exceptions;

public class InsufficientQuantityToDecreaseStockException : Exception
{
    public InsufficientQuantityToDecreaseStockException(long productId, long quantity) : base(
        $"Insufficient quantity to decreaseStock of product {productId} by {quantity}")
    {
    }
}