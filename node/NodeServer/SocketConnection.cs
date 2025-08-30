using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Node;
using NodeServer.stream;

namespace NodeServer;

public partial class SocketConnection : ISocketConnection, IDisposable
{
    private readonly ILogger logger;
    private readonly NodeServerOptions options;
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly Socket socket;
    private Socket? handler;
    private NetworkStream? stream;
    private RequestReader? reader;
    private NodeEventWriter? writer;

    public SocketConnection(ILogger<SocketConnection> logger, IOptions<NodeServerOptions> options, IHostApplicationLifetime applicationLifetime)
    {
        this.logger = logger;
        this.options = options.Value;
        this.applicationLifetime = applicationLifetime;
        string ip = "127.0.0.1";
        int port = this.options.Port;
        IPAddress addr = IPAddress.Parse(ip);
        IPEndPoint endpoint = new(addr, port);

        socket = new(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        LogBindMessage(ip, port);
        socket.Bind(endpoint);
        socket.Listen(100);
    }

    public async Task<Request> GetRequestAsync(CancellationToken token)
    {
        try
        {
            await Connect();
            return await reader!.ReadRequestAsync(token);
        }
        catch (Exception ex) when (ex is EndOfStreamException || ex is IOException)
        {
            // connection ended
            Disconnect();
            return await GetRequestAsync(token);
        }
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
            if (writer == null)
            {
                DebugFailedNodeEvent(nodeEvent.EventTypeCase);
            }
            else
            {
                await writer.WriteNodeEventAsync(nodeEvent, token);
            }
        }
        catch (Exception ex) when (ex is EndOfStreamException || ex is IOException)
        {
            DebugFailedNodeEvent(nodeEvent.EventTypeCase);
        }
    }

    private async Task Connect()
    {
        if (handler != null) return;
        // wait for someone to connect
        handler = await socket.AcceptAsync();
        stream = new(handler);
        reader = new(stream);
        writer = new(stream);
        LogClientConnect();
    }

    private void Disconnect()
    {
        stream?.Dispose();
        stream = null;
        handler?.Dispose();
        handler = null;
        reader = null;
        writer = null;
        LogClientDisconnect();
        return;
        // for when we have options to decide on reconnect behaviour
        applicationLifetime.StopApplication();
        throw new OperationCanceledException();
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "Binding to {Addr}:{Port}")]
    private partial void LogBindMessage(string addr, int port);

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Connected")]
    private partial void LogClientConnect();

    [LoggerMessage(Level = LogLevel.Information, Message = "Client Disconnected")]
    private partial void LogClientDisconnect();

    [LoggerMessage(Level = LogLevel.Debug, Message = "Failed to send NodeEvent: {Type}")]
    private partial void DebugFailedNodeEvent(NodeEvent.EventTypeOneofCase type);
}