using Microsoft.Extensions.Logging;
using Node;

namespace NodeServer.handlers;

public abstract class RequestHandler : IRequestHandler
{
    protected readonly ILogger logger;

    public abstract Request.RequestTypeOneofCase RequestType { get; }

    public RequestHandler(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger(GetType());
    }

    public abstract Task Handle(Request request);
}