using Node;

namespace NodeServer;

public interface ISocketConnection
{
    public Task<Request> GetRequestAsync(CancellationToken token);

    public Task SendNodeEventAsync(NodeEvent nodeEvent, CancellationToken token);
}