using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Moq;
using Node;

namespace NodeServer.Tests;

public sealed class RequestQueueTests : IDisposable
{
    private class TestQueue(ILogger<RequestQueue> logger) : RequestQueue(logger)
    {
        public Channel<Request> Queue { get => queue; }
    }

    private readonly TestQueue requestQueue;
    private readonly Mock<ILogger<RequestQueue>> loggerMock = new();

    public RequestQueueTests()
    {
        requestQueue = new(loggerMock.Object);
    }

    public void Dispose()
    {
        loggerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void DequeueAsync()
    {
        Request expected = new();
        await requestQueue.Queue.Writer.WriteAsync(expected);
        Request actual = await requestQueue.DequeueAsync(CancellationToken.None);
        Assert.Equal(expected, actual);
        loggerMock.Verify(m => m.IsEnabled(LogLevel.Debug));
    }

    [Fact]
    public async void EnqueueAsync()
    {
        Request expected = new();
        await requestQueue.EnqueueAsync(expected, CancellationToken.None);
        Request actual = await requestQueue.Queue.Reader.ReadAsync();
        Assert.Equal(expected, actual);
        loggerMock.Verify(m => m.IsEnabled(LogLevel.Debug));
    }
}