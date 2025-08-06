using Google.Protobuf;
using Node;
using NodeServer.stream;

namespace NodeServer.Tests.stream;

public class RequestReaderTests
{
    private readonly CancellationTokenSource cancel = new();

    [Fact]
    public async Task ReadRequestAsync_Success()
    {
        Request request = new()
        {
            ControlRequest = new ControlRequest()
            {
                Shutdown = new Shutdown()
                {
                    Delay = 22,
                },
            },
        };
        byte[] bytes = request.ToByteArray();
        using MemoryStream stream = new();
        using BinaryWriter writer = new(stream);
        writer.Write(bytes.Length);
        stream.Write(bytes);
        stream.Seek(0, SeekOrigin.Begin);
        RequestReader reader = new(stream);
        Request actual = await reader.ReadRequestAsync(cancel.Token);
        Assert.Equal(Request.RequestTypeOneofCase.ControlRequest, actual.RequestTypeCase);
        Assert.Equal(ControlRequest.ControlTypeOneofCase.Shutdown, actual.ControlRequest.ControlTypeCase);
        Assert.Equal((uint)22, actual.ControlRequest.Shutdown.Delay);
    }

    [Fact]
    public async Task ReadRequestAsync_Cancel()
    {
        using SlowStream stream = new();
        RequestReader reader = new(stream);
        Task task = reader.ReadRequestAsync(cancel.Token);
        await cancel.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
    }
}