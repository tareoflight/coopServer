using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Node;
using NodeServer.handlers;

namespace NodeServer;

public partial class RequestDispatcher(ILogger<RequestDispatcher> logger, IHandlerMap requestMap, IAsyncQueue<Request> requestQueue) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            DebugWaitRequest();
            Request request = await requestQueue.DequeueAsync(cancellationToken);
            DebugRequestType(request.RequestTypeCase);
            IRequestHandler? handler = requestMap.GetHandlerOrNull(request.RequestTypeCase);
            if (handler != null)
            {
                await handler.Handle(request);
            }
            else
            {
                WarnUnknownRequest(request.RequestTypeCase);
            }
        }
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Waiting for request")]
    private partial void DebugWaitRequest();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Dequeued Request: {RequestType}")]
    private partial void DebugRequestType(Request.RequestTypeOneofCase requestType);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Unknown Request Type '{RequestType}'")]
    private partial void WarnUnknownRequest(Request.RequestTypeOneofCase requestType);
}