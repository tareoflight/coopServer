namespace NodeServer;

public interface IAsyncQueue<T>
{
    public Task<T> DequeueAsync(CancellationToken cancellationToken);

    public Task EnqueueAsync(T item, CancellationToken cancellationToken);
}