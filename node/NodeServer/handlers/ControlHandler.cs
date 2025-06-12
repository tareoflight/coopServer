using Microsoft.Extensions.Logging;
using Node;

namespace NodeServer.handlers;

public partial class ControlHandler(ILoggerFactory loggerFactory) : RequestHandler(loggerFactory)
{
    public override Request.RequestTypeOneofCase RequestType => Request.RequestTypeOneofCase.Control;

    // might need to make async in the future
    public event EventHandler? Shutdown;

    public override async Task Handle(Request request)
    {
        logger.DebugProto(request);
        switch (request.Control.ControlTypeCase)
        {
            case ControlRequest.ControlTypeOneofCase.Shutdown:
                if (request.Control.Shutdown.HasDelay)
                {
                    await Task.Delay((int)request.Control.Shutdown.Delay);
                }
                Shutdown?.Invoke(this, new EventArgs());
                break;
            default:
                WarnUnknownControl(request.Control.ControlTypeCase);
                break;
        }
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Unknown Control Type: '{ControlType}'")]
    private partial void WarnUnknownControl(ControlRequest.ControlTypeOneofCase controlType);
}