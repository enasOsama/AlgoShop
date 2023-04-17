using Orders.Domain.Exceptions;

namespace Orders.Domain.Order_Aggregate;

public class Order
{
    private readonly List<OrderItem> _orderItems;

    public Order(long userId)
    {
        UserId = userId;
        Id = DateTime.UtcNow.Ticks;
        _orderItems = new List<OrderItem>();
        Status = OrderStatus.Draft;
        CreatedAt = DateTime.UtcNow;
    }

    public long UserId { get; }

    public long Id { get; }

    public DateTime CreatedAt { get; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

    public OrderStatus Status { get; private set; }

    public void SetAsApproved()
    {
        if (Status != OrderStatus.Draft)
        {
            throw new OrderStateInvalid(Status, OrderStatus.Approved);
        }

        Status = OrderStatus.Approved;
    }

    public void SetAsCancelled()
    {
        if (Status != OrderStatus.Draft)
        {
            throw new OrderStateInvalid(Status, OrderStatus.Cancelled);
        }

        Status = OrderStatus.Cancelled;
    }

    public void AddOrderItem(long productId, int quantity)
    {
        _orderItems.Add(new OrderItem(productId, quantity));
    }
}