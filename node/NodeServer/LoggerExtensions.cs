using Microsoft.Extensions.Logging;

namespace NodeServer;

public static partial class LoggerExtensions
{
    [LoggerMessage(Level = LogLevel.Debug, Message = "{Request}")]
    public static partial void DebugProto(this ILogger logger, object request);
}