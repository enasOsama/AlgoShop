// https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/grpc/test-services/sample/Tests/Server/IntegrationTests/Helpers/GrpcTestContext.cs

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Orders.API.Tests.Infrastructure.Helpers;

internal class GrpcTestContext<TStartup> : IDisposable where TStartup : class
{
    private readonly GrpcTestFixture<TStartup> _fixture;
    private readonly ITestOutputHelper _outputHelper;
    private readonly Stopwatch _stopwatch;

    public GrpcTestContext(GrpcTestFixture<TStartup> fixture, ITestOutputHelper outputHelper)
    {
        _stopwatch = Stopwatch.StartNew();
        _fixture = fixture;
        _outputHelper = outputHelper;
        _fixture.LoggedMessage += WriteMessage;
    }

    public void Dispose()
    {
        _fixture.LoggedMessage -= WriteMessage;
    }

    private void WriteMessage(LogLevel logLevel, string category, EventId eventId, string message, Exception? exception)
    {
        var log = $"{_stopwatch.Elapsed.TotalSeconds:N3}s {category} - {logLevel}: {message}";
        if (exception != null) log += Environment.NewLine + exception;
        _outputHelper.WriteLine(log);
    }
}