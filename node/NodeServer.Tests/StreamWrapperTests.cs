namespace NodeServer.Tests;

public class StreamWrapperTests
{
    private readonly CancellationTokenSource cancel = new();
    private readonly MemoryStream stream = new();
    private readonly BinaryReader reader;
    private readonly BinaryWriter writer;
    private readonly StreamWrapper wrapper;

    public StreamWrapperTests()
    {
        reader = new(stream);
        writer = new(stream);
        wrapper = new(stream);
    }

    [Fact]
    public async Task GetBytesAsync()
    {
        stream.Write(Enumerable.Repeat<byte>(55, 10).ToArray());
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal([55, 55, 55, 55, 55], await wrapper.GetBytesAsync(5, cancel.Token));
    }

    [Fact]
    public async Task ReadSizeAsync()
    {
        writer.Write(1234);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(1234, await wrapper.ReadSizeAsync(cancel.Token));
    }

    [Fact]
    public async Task WriteSizeAsync()
    {
        await wrapper.WriteSizeAsync(1234, cancel.Token);
        stream.Seek(0, SeekOrigin.Begin);
        Assert.Equal(1234, reader.ReadInt32());
    }
}