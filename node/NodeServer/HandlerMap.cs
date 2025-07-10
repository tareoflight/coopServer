using Node;
using NodeServer.handlers;

namespace NodeServer;

public class HandlerMap : IHandlerMap
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

    public IRequestHandler? GetHandlerOrNull(Request.RequestTypeOneofCase requestType)
    {
        if (handlers.TryGetValue(requestType, out IRequestHandler? handler))
        {
            return handler;
        }
        else
        {
            return null;
        }
    }
}