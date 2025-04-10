namespace NodeServer;

public interface ISocket
{
    public Task<int> ReceiveAsync(ArraySegment<byte> buffer);
}