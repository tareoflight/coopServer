using Google.Protobuf;
using Node;

namespace NodeServer.Tests;

public class RequestReaderTests
{
    private readonly CancellationTokenSource cancel = new();
    private readonly MemoryStream stream = new();
    private readonly BinaryWriter writer;
    private readonly RequestReader reader;
    private readonly StreamWrapper wrapper;

    public RequestReaderTests()
    {
        writer = new(stream);
        reader = new(stream);
        wrapper = new(stream);
    }

    [Fact]
    public async Task GetRequest_Control_Shutdown()
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
        writer.Write(bytes.Length);
        stream.Write(bytes);
        stream.Seek(0, SeekOrigin.Begin);
        Request actual = await reader.GetRequestAsync(cancel.Token);
        Assert.Equal(Request.RequestTypeOneofCase.ControlRequest, actual.RequestTypeCase);
        Assert.Equal(ControlRequest.ControlTypeOneofCase.Shutdown, actual.ControlRequest.ControlTypeCase);
        Assert.Equal((uint)22, actual.ControlRequest.Shutdown.Delay);
    }
}