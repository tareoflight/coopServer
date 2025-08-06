using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Node;

namespace NodeServer;

public class ReceiveService(ILogger<ReceiveService> logger, ISocketConnection socketConnection, IAsyncQueue<Request> requestQueue) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.InfoStarting();
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Request request = await socketConnection.GetRequestAsync(stoppingToken);
                await requestQueue.EnqueueAsync(request, stoppingToken);
            }
        }
        catch (OperationCanceledException) { } // from the exitSource
    }
}