using Grpc.Core;
using Stocks.Domain.Exceptions;
using Stocks.Domain.Stock_Aggregate;

namespace Stocks.API.Services;

public class StocksService : Stocks.StocksBase
{
    private readonly ILogger<StocksService> _logger;
    private readonly IStockRepository _stockRepository;

    public StocksService(ILogger<StocksService> logger, IStockRepository stockRepository)
    {
        _logger = logger;
        _stockRepository = stockRepository;
    }


    public override async Task<StockResponse?> GetStock(StockRequest request, ServerCallContext context)
    {
        var response = new StockResponse();

        foreach (var requestItem in request.Items)
        {
            var stock = await _stockRepository.GetByProductIdOrDefaultAsync(requestItem.ProductId);

            if (stock is null)
            {
                context.Status = new Status(StatusCode.NotFound, $"Product with ID '{requestItem.ProductId}' not found.");
                _logger.LogInformation($"Product with ID '{requestItem.ProductId}' not found.");
                return null;
            }

            context.Status = new Status(StatusCode.OK, null);
            response.Items.Add(new StockResponseItem
            {
                ProductId = stock.ProductId,
                HasStock = requestItem.Quantity <= stock.Quantity
            });
        }

        return response;
    }

    public override async Task<StockResponse> IncreaseStock(StockRequest request, ServerCallContext context)
    {
        var response = new StockResponse();

        foreach (var requestItem in request.Items)
        {
            var stock = await _stockRepository.GetByProductIdOrDefaultAsync(requestItem.ProductId);

            if (stock is null)
            {
                stock = new Stock(requestItem.ProductId);
                stock.IncreaseStock(requestItem.Quantity);
                _logger.LogInformation(
                    $"Stock for product '{requestItem.ProductId}' increased by {requestItem.Quantity}.");

                await _stockRepository.AddAsync(stock);
            }
            else
            {
                stock.IncreaseStock(requestItem.Quantity);
                _logger.LogInformation(
                    $"Stock for product '{requestItem.ProductId}' increased by {requestItem.Quantity}.");

                await _stockRepository.UpdateAsync(stock);
            }

            response.Items.Add(new StockResponseItem
            {
                ProductId = stock.ProductId,
                HasStock = true
            });
        }

        return response;
    }

    public override async Task<StockResponse?> DecreaseStock(StockRequest request, ServerCallContext context)
    {
        var response = new StockResponse();

        var stocksToUpdate = new List<Stock>();

        foreach (var requestItem in request.Items)
        {
            var stock = await _stockRepository.GetByProductIdOrDefaultAsync(requestItem.ProductId);

            if (stock is null)
            {
                context.Status = new Status(StatusCode.NotFound,
                    $"Product with ID '{requestItem.ProductId}' not found.");
                _logger.LogInformation($"Product with ID '{requestItem.ProductId}' not found.");
                return null;
            }

            try
            {
                stock.DecreaseStock(requestItem.Quantity);
            }
            catch (Exception e) when (e is QuantityMustBePositiveException or InsufficientQuantityToDecreaseStockException)
            {
                context.Status = new Status(StatusCode.FailedPrecondition, e.Message);
                _logger.LogError(e.Message);
                return null;
            }

            stocksToUpdate.Add(stock);
            _logger.LogInformation($"Stock for product '{requestItem.ProductId}' decreased by {requestItem.Quantity}.");
        }


        //Better to implement a WriteModel for the unit of work 
        //Revert changes in case of error
        foreach (var stock in stocksToUpdate)
        {
            await _stockRepository.UpdateAsync(stock);
            response.Items.Add(new StockResponseItem
            {
                ProductId = stock.ProductId,
                HasStock = true
            });
        }

        return response;
    }
}