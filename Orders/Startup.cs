using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Orders.API.Configurations;
using Orders.API.Services;
using Orders.Domain;
using static Stocks.API.Stocks;

namespace Orders.API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

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

        services.AddGrpcClient<StocksClient>(options =>
        {
            options.Address = new Uri(Configuration["StocksServiceUrl"]);
        });

        services.AddControllers();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGrpcService<OrderService>();
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