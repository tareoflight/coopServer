namespace NodeServer;

public class StreamWrapper(Stream stream)
{
    private readonly Stream stream = stream;

    public async Task<byte[]> GetBytesAsync(int len)
    {
        byte[] buffer = new byte[len];
        await stream.ReadExactlyAsync(buffer);
        return buffer;
    }

    public async Task<int> ReadSizeAsync()
    {
        byte[] bytes = await GetBytesAsync(4);
        return BitConverter.ToInt32(bytes);
    }

    public async Task WriteSizeAsync(int size)
    {
        await stream.WriteAsync(BitConverter.GetBytes(size));
    }
}