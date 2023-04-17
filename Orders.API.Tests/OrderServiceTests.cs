using FluentAssertions;
using Orders.API.Tests.Infrastructure;
using Orders.API.Tests.Infrastructure.Helpers;
using Stocks.API;
using Stocks.Domain.Stock_Aggregate;
using Xunit;
using Xunit.Abstractions;
using Assert = Xunit.Assert;

namespace Orders.API.Tests;

public class OrderServiceTests : IntegrationTestBase
{
    public OrderServiceTests(GrpcTestFixture<Startup> fixture, ITestOutputHelper outputHelper) :
        base(fixture, outputHelper)
    {

    }

    [Fact]
    public async Task PlaceOrder_ShouldReturnOrderId_WhenOrderIsValid()
    {
        // Arrange
        var orderServiceClient = new Orders.OrdersClient(Channel);
        var orderRequest = new OrderRequest
        {
            UserId = DateTime.UtcNow.Ticks,
            Items =
            {
                new OrderItem
                {
                    ProductId = 1,
                    Quantity = 2
                }
            }
        };

        // Act
        var response = await orderServiceClient.PlaceOrderAsync(orderRequest);

        // Assert
        response.Should().NotBeNull();
        response.OrderId.Should().BeGreaterThan(0);
        response.Status.Should().Be(OrderStatus.Approved);
        //Ideally should also check if the stock was decreased by mocking the stock service or calling a function to return the stock
    }

    [Fact]
    public async Task PlaceOrder_ShouldCancelOrder_WhenOrderHasOutOfStockProduct()
    {
        // Arrange
        var orderServiceClient = new Orders.OrdersClient(Channel);
        var orderRequest = new OrderRequest
        {
            UserId = DateTime.UtcNow.Ticks,
            Items =
            {
                new OrderItem
                {
                    ProductId = 1,
                    Quantity = 20
                },
                new OrderItem
                {
                    ProductId = 2,
                    //Ideally the value should not be "magical number" but should be a constant or a function that returns the stock
                    Quantity = 100 //This product is out of stock
                }
            }
        };

        // Act
        var response = await orderServiceClient.PlaceOrderAsync(orderRequest);

        // Assert
        response.Should().NotBeNull();
        response.OrderId.Should().BeGreaterThan(0);
        response.Status.Should().Be(OrderStatus.Cancelled);
        //Ideally should also check if the stock was not decreased by mocking the stock service or calling a function to return the stock
    }
}