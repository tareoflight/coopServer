using Node;

namespace NodeServer.stream;

public class RequestReader(Stream stream)
{
    protected readonly Stream stream = stream;

    public async Task<Request> ReadRequestAsync(CancellationToken token)
    {
        int size = await ReadSizeAsync(token);
        return Request.Parser.ParseFrom(await ReadBytesAsync(size, token));
    }

    protected async Task<byte[]> ReadBytesAsync(int len, CancellationToken token)
    {
        byte[] buffer = new byte[len];
        await stream.ReadExactlyAsync(buffer, token);
        return buffer;
    }

    protected async Task<int> ReadSizeAsync(CancellationToken token)
    {
        byte[] bytes = await ReadBytesAsync(4, token);
        return BitConverter.ToInt32(bytes);
    }
}