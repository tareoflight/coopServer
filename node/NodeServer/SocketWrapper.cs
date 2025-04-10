namespace NodeServer;

public class SocketWrapper(ISocket socket)
{
    private readonly ISocket socket = socket;

    public async Task<byte[]> GetBytes(int count)
    {
        byte[] bytes = new byte[count];
        int read = 0;
        do
        {
            byte[] buffer = new byte[Math.Min(1_024, count - read)];
            int received = await socket.ReceiveAsync(buffer);
            Array.Copy(buffer, 0, bytes, read, received);
            read += received;
        } while (read < count);
        return bytes;
    }

    public async Task<uint> GetUint()
    {
        return BitConverter.ToUInt32(await GetBytes(4));
    }
}