using Google.Protobuf;
using Node;

namespace NodeServer.Tests;

public class RequestReaderTests
{
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
            Control = new ControlRequest()
            {
                Shutdown = new Shutdown()
                {
                    Delay = 22,
                },
            },
            RequestId = 111,
        };
        byte[] bytes = request.ToByteArray();
        writer.Write(bytes.Length);
        stream.Write(bytes);
        stream.Seek(0, SeekOrigin.Begin);
        Request actual = await reader.GetRequestAsync();
        Assert.Equal((uint)111, actual.RequestId);
        Assert.Equal(Request.RequestTypeOneofCase.Control, actual.RequestTypeCase);
        Assert.Equal(ControlRequest.ControlTypeOneofCase.Shutdown, actual.Control.ControlTypeCase);
        Assert.Equal((uint)22, actual.Control.Shutdown.Delay);
    }
}