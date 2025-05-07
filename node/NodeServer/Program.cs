namespace NodeServer;

using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Node;

public partial class Program
{
    static async Task Main(string[] args)
    {
        ILogger logger = CreateLogger();
        LogStartupMessage(logger);

        using Socket socket = Bind(logger, "127.0.0.1", 25569);
        socket.Listen(100);

        while (true)
        {
            // wait for someone to connect
            using Socket handler = await socket.AcceptAsync();
            using NetworkStream stream = new(handler);
            RequestReader reader = new(stream);
            LogClientConnect(logger);

            try
            {
                while (true)
                {
                    Request request = await reader.GetRequestAsync();
                    LogRequest(logger, request);
                    switch (request.RequestTypeCase)
                    {
                        case Request.RequestTypeOneofCase.Control:
                            request.ToString();
                            break;
                        case Request.RequestTypeOneofCase.None:
                            LogEmptyRequest(logger);
                            break;
                    }
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

    static ILogger CreateLogger()
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug));
        return loggerFactory.CreateLogger("Main");
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

    [LoggerMessage(Level = LogLevel.Debug, Message = "Request: {Request}")]
    static partial void LogRequest(ILogger logger, Request request);

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Connected")]
    static partial void LogClientConnect(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Disconnected")]
    static partial void LogClientDisconnect(ILogger logger);
}