using Node;

namespace NodeServer;

public interface IRequestQueue
{
    public Task<Request> DequeueAsync(CancellationToken cancellationToken);

    public Task EnqueueAsync(Request request, CancellationToken cancellationToken);
}