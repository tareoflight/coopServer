using Google.Protobuf;
using Node;
using NodeServer.stream;

namespace NodeServer.Tests.stream;

public sealed class NodeEventWriterTests
{
    private readonly CancellationTokenSource tokenSource = new();

    [Fact]
    public async Task WriteNodeEventAsync_Success()
    {
        NodeEvent nodeEvent = new()
        {
            HeartbeatEvent = new() { Status = 111 }
        };
        byte[] expected = nodeEvent.ToByteArray();
        using MemoryStream stream = new();
        NodeEventWriter writer = new(stream);
        await writer.WriteNodeEventAsync(nodeEvent, tokenSource.Token);
        stream.Seek(0, SeekOrigin.Begin);
        using BinaryReader reader = new(stream);
        int size = reader.ReadInt32();
        byte[] bytes = reader.ReadBytes(size);
        Assert.Equal(expected.Length, size);
        Assert.Equal(expected, bytes);
        Assert.Equal(stream.Length, stream.Position);
    }

    [Fact]
    public async Task WriteNodeEventAsync_Cancel()
    {
        using SlowStream stream = new();
        NodeEventWriter writer = new(stream);
        Task task = writer.WriteNodeEventAsync(new NodeEvent(), tokenSource.Token);
        await tokenSource.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
    }
}