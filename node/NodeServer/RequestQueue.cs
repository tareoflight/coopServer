using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Node;

namespace NodeServer;

public partial class RequestQueue(ILogger<RequestQueue> logger) : IRequestQueue
{
    protected readonly Channel<Request> queue = Channel.CreateUnbounded<Request>
    (
        new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = true,
        }
    );

    public async Task<Request> DequeueAsync(CancellationToken cancellationToken)
    {
        await queue.Reader.WaitToReadAsync(cancellationToken);
        Request request = await queue.Reader.ReadAsync(cancellationToken);
        DebugDequeueRequest(request.RequestTypeCase);
        return request;
    }

    public Task EnqueueAsync(Request request, CancellationToken cancellationToken)
    {
        DebugQueueRequest(request.RequestTypeCase);
        return queue.Writer.WriteAsync(request, cancellationToken).AsTask();
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Queueing request: {RequestType}")]
    private partial void DebugQueueRequest(Request.RequestTypeOneofCase requestType);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Dequeue request: {RequestType}")]
    private partial void DebugDequeueRequest(Request.RequestTypeOneofCase requestType);
}