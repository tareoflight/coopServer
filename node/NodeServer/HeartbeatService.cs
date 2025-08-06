using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Node;

namespace NodeServer;

public partial class HeartbeatService(ILogger<HeartbeatService> logger, ISocketConnection socketConnection) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.InfoStarting();
        while (!stoppingToken.IsCancellationRequested)
        {
            DebugSending();
            await socketConnection.SendNodeEventAsync(new NodeEvent() { HeartbeatEvent = new Heartbeat() { Status = 200 } }, stoppingToken);
            await Task.Delay(5000, stoppingToken);
        }
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Sending heartbeat")]
    private partial void DebugSending();
}