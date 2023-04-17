using Microsoft.Extensions.DependencyInjection;
using Stocks.Domain.Stock_Aggregate;

namespace Stocks.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IStockRepository, StockRepository>();
        return services;
    }
}