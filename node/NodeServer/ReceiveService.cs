using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Node;

namespace NodeServer;

public partial class ReceiveService(ILogger<ReceiveService> logger, IAsyncQueue<Request> requestQueue) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        string ip = "127.0.0.1";
        int port = 25569;
        IPAddress addr = IPAddress.Parse(ip);
        IPEndPoint endpoint = new(addr, port);

        using Socket socket = new(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        LogBindMessage(ip, port);
        socket.Bind(endpoint);
        socket.Listen(100);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // wait for someone to connect
                using Socket handler = await socket.AcceptAsync(stoppingToken);
                using NetworkStream stream = new(handler);
                RequestReader reader = new(stream);
                LogClientConnect();

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        Request request = await reader.GetRequestAsync(stoppingToken);
                        await requestQueue.EnqueueAsync(request, stoppingToken);
                    }
                }
                catch (Exception ex) when
                (
                    ex is EndOfStreamException ||
                    ex is IOException
                )
                {
                    // connection ended, loop around for the next
                    LogClientDisconnect();
                }
            }
        }
        catch (OperationCanceledException) { } // from the exitSource
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Binding to {Addr}:{Port}")]
    private partial void LogBindMessage(string addr, int port);

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Connected")]
    private partial void LogClientConnect();

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Disconnected")]
    private partial void LogClientDisconnect();
}