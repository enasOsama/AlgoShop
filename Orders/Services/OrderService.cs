using Grpc.Core;
using Orders.Domain.Order_Aggregate;
using Stocks.API;

namespace Orders.API.Services;

public class OrderService : API.Orders.OrdersBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly Stocks.API.Stocks.StocksClient _stockClient;

    public OrderService(IOrderRepository orderRepository, Stocks.API.Stocks.StocksClient stockClient)
    {
        _orderRepository = orderRepository;
        _stockClient = stockClient;
    }

    public override async Task<OrderResponse> PlaceOrder(OrderRequest request, ServerCallContext context)
    {
        var order = new Order(request.UserId);
        foreach (var item in request.Items) order.AddOrderItem(item.ProductId, item.Quantity);

        await _orderRepository.AddAsync(order);

        var stockRequest = MapToStockRequest(request);
        try
        {
            var productsStockInformation = await _stockClient.GetStockAsync(stockRequest);

            if (productsStockInformation.Items.Any(x => !x.HasStock))
            {
                order.SetAsCancelled();
            }
            else
            {
                await _stockClient.DecreaseStockAsync(stockRequest);
                order.SetAsApproved();
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
        {
            order.SetAsCancelled();
        }

        await _orderRepository.UpdateAsync(order);

        return new OrderResponse
        {
            OrderId = order.Id,
            Status = MapToOrderStatus(order)
        };
    }

    private static StockRequest MapToStockRequest(OrderRequest request)
    {
        var stockRequest = new StockRequest();

        stockRequest.Items.AddRange(request.Items.Select(item => new StockRquestItem
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity
        }));
        return stockRequest;
    }

    private static OrderStatus MapToOrderStatus(Order order)
    {
        return order.Status switch
        {
            Domain.Order_Aggregate.OrderStatus.Draft => OrderStatus.Draft,
            Domain.Order_Aggregate.OrderStatus.Approved => OrderStatus.Approved,
            Domain.Order_Aggregate.OrderStatus.Cancelled => OrderStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}