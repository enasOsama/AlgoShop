using Orders.Domain.Order_Aggregate;

namespace Orders.Domain.Exceptions;

public class OrderStateInvalid : Exception
{
    public OrderStateInvalid(OrderStatus currentStatus, OrderStatus targetStatus) :
        base($"Order status cannot be changed from {targetStatus} to {currentStatus}")
    {
    }
}