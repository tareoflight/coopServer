using System.Threading.Channels;

namespace NodeServer;

public partial class AsyncQueue<T>() : IAsyncQueue<T>
{
    protected readonly Channel<T> queue = Channel.CreateUnbounded<T>
    (
        new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = true,
        }
    );

    public async Task<T> DequeueAsync(CancellationToken cancellationToken)
    {
        await queue.Reader.WaitToReadAsync(cancellationToken);
        return await queue.Reader.ReadAsync(cancellationToken);
    }

    public Task EnqueueAsync(T request, CancellationToken cancellationToken)
    {
        return queue.Writer.WriteAsync(request, cancellationToken).AsTask();
    }
}