using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Node;

namespace NodeServer;

public partial class RequestQueue(ILogger logger, IRequestDispatcher dispatcher)
{
    private readonly Channel<Request> queue = Channel.CreateUnbounded<Request>
    (
        new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = true,
        }
    );

    public ValueTask EnqueueRequest(Request request, CancellationToken stoppingToken)
    {
        DebugQueueRequest(request.RequestTypeCase);
        return queue.Writer.WriteAsync(request, stoppingToken);
    }

    public async Task DequeueLoop(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            DebugWaitRequest();
            await queue.Reader.WaitToReadAsync(stoppingToken);
            Request request = await queue.Reader.ReadAsync(stoppingToken);
            DebugDequeueRequest(request.RequestTypeCase);
            await dispatcher.Dispatch(request);
        }
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Waiting for request")]
    private partial void DebugWaitRequest();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Queueing request: {RequestType}")]
    private partial void DebugQueueRequest(Request.RequestTypeOneofCase requestType);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Dequeue request: {RequestType}")]
    private partial void DebugDequeueRequest(Request.RequestTypeOneofCase requestType);
}