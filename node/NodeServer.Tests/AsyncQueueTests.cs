using System.Threading.Channels;

namespace NodeServer.Tests;

public sealed class AsyncQueueTests
{
    private class TestQueue() : AsyncQueue<object>
    {
        public Channel<object> Queue { get => queue; }
    }

    private readonly TestQueue requestQueue;

    public AsyncQueueTests()
    {
        requestQueue = new();
    }

    [Fact]
    public async void DequeueAsync()
    {
        object expected = new();
        await requestQueue.Queue.Writer.WriteAsync(expected);
        object actual = await requestQueue.DequeueAsync(CancellationToken.None);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async void EnqueueAsync()
    {
        object expected = new();
        await requestQueue.EnqueueAsync(expected, CancellationToken.None);
        object actual = await requestQueue.Queue.Reader.ReadAsync();
        Assert.Equal(expected, actual);
    }
}