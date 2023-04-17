using Microsoft.Extensions.DependencyInjection;
using Orders.Domain.Order_Aggregate;

namespace Orders.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }
}