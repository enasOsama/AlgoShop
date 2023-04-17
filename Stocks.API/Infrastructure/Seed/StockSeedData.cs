using MongoDB.Driver;
using Stocks.Domain.Stock_Aggregate;

namespace Stocks.API.Infrastructure.Seed;

internal class StockSeedData
{
    private readonly IMongoDatabase _database;

    public StockSeedData(IMongoDatabase database)
    {
        _database = database;
    }

    public void Seed()
    {
        var stocks = _database.GetCollection<Stock>("stocks");

        var hasData = stocks.FindSync(s => s.ProductId == 1 || s.ProductId == 2 || s.ProductId == 3).Any();
        if (hasData)
        {
            return;
        }

        var stock1 = new Stock(1);
        stock1.IncreaseStock(50);


        var stock2 = new Stock(2);
        stock2.IncreaseStock(10);


        var stock3 = new Stock(3);
        stock3.IncreaseStock(10);


        var list = new List<Stock>
        {
            stock1, stock2, stock3
        };

        stocks.InsertMany(list);
    }
}