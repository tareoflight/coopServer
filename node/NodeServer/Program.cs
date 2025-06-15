namespace NodeServer;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Node;
using NodeServer.handlers;

public partial class Program
{
    static async Task Main(string[] args)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        ILogger logger = loggerFactory.CreateLogger("Main");
        LogStartupMessage(logger);

        CancellationTokenSource exitSource = new();
        RequestDispatcher requestDispatcher = new(loggerFactory.CreateLogger(typeof(RequestDispatcher)));
        RequestQueue requestQueue = new(loggerFactory.CreateLogger(typeof(RequestQueue)), requestDispatcher);
        ControlHandler controlHandler = new(loggerFactory);

        controlHandler.Shutdown += (_, _) => exitSource.Cancel();
        requestDispatcher.AddHandler(controlHandler);

        // start the dequeue loop
        Task requestLoop = requestQueue.DequeueLoop(exitSource.Token);

        using Socket socket = Bind(logger, "127.0.0.1", 25569);
        socket.Listen(100);

        try
        {
            while (!exitSource.IsCancellationRequested)
            {
                // wait for someone to connect
                using Socket handler = await socket.AcceptAsync(exitSource.Token);
                using NetworkStream stream = new(handler);
                RequestReader reader = new(stream);
                LogClientConnect(logger);

                try
                {
                    while (!exitSource.IsCancellationRequested)
                    {
                        Request request = await reader.GetRequestAsync(exitSource.Token);
                        await requestQueue.EnqueueRequest(request, exitSource.Token);
                    }
                }
                catch (Exception ex) when
                (
                    ex is EndOfStreamException ||
                    ex is IOException
                )
                {
                    // connection ended, loop around for the next
                    LogClientDisconnect(logger);
                }
            }
        }
        catch (OperationCanceledException) { } // from the exitSource
        InfoShutdown(logger);
        await requestLoop;
    }

    static Socket Bind(ILogger logger, string ip, int port)
    {
        IPAddress addr = IPAddress.Parse(ip);
        IPEndPoint endpoint = new(addr, port);

        Socket socket = new(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        LogBindMessage(logger, ip, port);
        socket.Bind(endpoint);
        return socket;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting Node Server")]
    static partial void LogStartupMessage(ILogger logger);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Binding to {Addr}:{Port}")]
    static partial void LogBindMessage(ILogger logger, string addr, int port);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Ignoring empty request")]
    static partial void LogEmptyRequest(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Connected")]
    static partial void LogClientConnect(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Disconnected")]
    static partial void LogClientDisconnect(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Shutting down")]
    static partial void InfoShutdown(ILogger logger);
}