namespace Stocks.Domain.Exceptions;

public class QuantityMustBePositiveException : Exception
{
    public QuantityMustBePositiveException(long quantity) : base(
        $"Cannot use negative number {quantity} to increase/decrease stocks")
    {
    }
}