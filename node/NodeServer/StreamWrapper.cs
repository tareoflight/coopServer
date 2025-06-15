namespace NodeServer;

public class StreamWrapper(Stream stream)
{
    private readonly Stream stream = stream;

    public async Task<byte[]> GetBytesAsync(int len, CancellationToken token)
    {
        byte[] buffer = new byte[len];
        await stream.ReadExactlyAsync(buffer, token);
        return buffer;
    }

    public async Task<int> ReadSizeAsync(CancellationToken token)
    {
        byte[] bytes = await GetBytesAsync(4, token);
        return BitConverter.ToInt32(bytes);
    }

    public async Task WriteSizeAsync(int size, CancellationToken token)
    {
        await stream.WriteAsync(BitConverter.GetBytes(size), token);
    }
}