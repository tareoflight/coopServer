using Microsoft.Extensions.Logging;

namespace NodeServer;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Starting...")]
    public static partial void InfoStarting(this ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "{Request}")]
    public static partial void DebugProto(this ILogger logger, object request);
}