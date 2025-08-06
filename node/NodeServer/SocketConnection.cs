using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Node;
using NodeServer.stream;

namespace NodeServer;

public partial class SocketConnection : ISocketConnection, IDisposable
{
    private readonly ILogger logger;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly Socket socket;
    private readonly Socket handler;
    private readonly NetworkStream stream;
    private readonly RequestReader reader;
    private readonly NodeEventWriter writer;

    public SocketConnection(ILogger<SocketConnection> logger, IHostApplicationLifetime applicationLifetime)
    {
        this.logger = logger;
        this.applicationLifetime = applicationLifetime;
        string ip = "127.0.0.1";
        int port = 25569;
        IPAddress addr = IPAddress.Parse(ip);
        IPEndPoint endpoint = new(addr, port);

        socket = new(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        LogBindMessage(ip, port);
        socket.Bind(endpoint);
        socket.Listen(100);
        // wait for someone to connect
        handler = socket.Accept();
        stream = new(handler);
        reader = new(stream);
        writer = new(stream);
        LogClientConnect();
    }

    public async Task<Request> GetRequestAsync(CancellationToken token)
    {
        try
        {
            return await reader!.ReadRequestAsync(token);
        }
        catch (Exception ex) when (ex is EndOfStreamException || ex is IOException)
        {
            // connection ended
            Disconnect();
        }
        throw new OperationCanceledException();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        stream?.Dispose();
        handler?.Dispose();
        socket?.Dispose();
    }

    public async Task SendNodeEventAsync(NodeEvent nodeEvent, CancellationToken token)
    {
        try
        {
            await writer!.WriteNodeEventAsync(nodeEvent, token);
            return;
        }
        catch (Exception ex) when (ex is EndOfStreamException || ex is IOException)
        {
            // connection ended
            Disconnect();
        }
        throw new OperationCanceledException();
    }

    private void Disconnect()
    {
        LogClientDisconnect();
        applicationLifetime.StopApplication();
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Binding to {Addr}:{Port}")]
    private partial void LogBindMessage(string addr, int port);

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Connected")]
    private partial void LogClientConnect();

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Disconnected")]
    private partial void LogClientDisconnect();
}