using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Stocks.API.Configurations;
using Stocks.API.Infrastructure.Seed;
using Stocks.API.Services;
using Stocks.Domain;
using Stocks.Domain.Stock_Aggregate;

namespace Stocks.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDomainServices();
        services.AddGrpc();

        services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDB"));
        services.AddSingleton(app =>
        {
            var mongoDbSettings = app.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = new MongoClient(mongoDbSettings.ConnectionUri);
            return client.GetDatabase(mongoDbSettings.DatabaseName);
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app)
    {
        var mongoDatabase = app.ApplicationServices.GetRequiredService<IMongoDatabase>();
        new StockSeedData(mongoDatabase).Seed();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<StocksService>();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync(
                    "Communication with gRPC endpoints must be made through a gRPC client. " +
                    "To learn how to create a client, visit: " +
                    "https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    }
}