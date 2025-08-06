using Google.Protobuf;
using Node;

namespace NodeServer.stream;

public class NodeEventWriter(Stream stream)
{
    protected readonly Stream stream = stream;

    public async Task WriteNodeEventAsync(NodeEvent nodeEvent, CancellationToken token)
    {
        byte[] bytes = nodeEvent.ToByteArray();
        await WriteSizeAsync(bytes.Length, token);
        await stream.WriteAsync(bytes, token);
    }

    protected async Task WriteSizeAsync(int size, CancellationToken token)
    {
        await stream.WriteAsync(BitConverter.GetBytes(size), token);
    }
}