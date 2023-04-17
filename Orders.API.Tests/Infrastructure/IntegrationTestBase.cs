//https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/grpc/test-services/sample/Tests/Server/IntegrationTests/IntegrationTestBase.cs

using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orders.API.Tests.Infrastructure.Helpers;
using Stocks.API.Infrastructure.Seed;
using Xunit;
using Xunit.Abstractions;

namespace Orders.API.Tests.Infrastructure;

public class IntegrationTestBase : IClassFixture<GrpcTestFixture<Startup>>, IDisposable
{
    private readonly IDisposable? _testContext;
    private GrpcChannel? _channel;

    public IntegrationTestBase(GrpcTestFixture<Startup> fixture, ITestOutputHelper outputHelper)
    {
        Fixture = fixture;
        var grpcTestFixture = new GrpcTestFixture<Stocks.API.Startup>();

        Fixture.ConfigureWebHost(builder =>
        {
            builder.ConfigureServices(
                services =>
                    services
                        .AddGrpcClient<Stocks.API.Stocks.StocksClient>(options =>
                        {
                            options.Address = grpcTestFixture.BaseAddress;
                        })
                        .ConfigurePrimaryHttpMessageHandler(() => grpcTestFixture.Handler));
        });

        _testContext = Fixture.GetTestContext(outputHelper);
    }

    protected GrpcTestFixture<Startup> Fixture { get; set; }

    protected ILoggerFactory LoggerFactory => Fixture.LoggerFactory;

    protected GrpcChannel Channel => _channel ??= CreateChannel();

    public void Dispose()
    {
        _testContext?.Dispose();
        _channel = null;
    }

    protected GrpcChannel CreateChannel()
    {
        return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            LoggerFactory = LoggerFactory,
            HttpHandler = Fixture.Handler
        });
    }
}