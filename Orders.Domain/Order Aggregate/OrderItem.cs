namespace Orders.Domain.Order_Aggregate;

public class OrderItem
{
    public OrderItem(long productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public long ProductId { get; }

    public int Quantity { get; }
}