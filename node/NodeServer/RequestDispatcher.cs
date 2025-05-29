using Microsoft.Extensions.Logging;
using Node;
using NodeServer.handlers;

namespace NodeServer;

public partial class RequestDispatcher(ILogger logger) : IRequestDispatcher
{
    private readonly Dictionary<Request.RequestTypeOneofCase, IRequestHandler> handlers = [];

    public void AddHandler(IRequestHandler handler)
    {
        if (handlers.ContainsKey(handler.RequestType))
        {
            throw new ArgumentException($"Already a handler for {handler.RequestType}", nameof(handler));
        }

        handlers.Add(handler.RequestType, handler);
    }

    public async Task Dispatch(Request request)
    {
        if (handlers.TryGetValue(request.RequestTypeCase, out IRequestHandler? handler))
        {
            await handler.Handle(request);
        }
        else
        {
            WarnUnknownRequest(request.RequestTypeCase);
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Unknown Request Type '{RequestType}'")]
    private partial void WarnUnknownRequest(Request.RequestTypeOneofCase requestType);
}