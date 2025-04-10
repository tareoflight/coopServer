using Moq;

namespace NodeServer.Tests;

public class SocketWrapperTests
{
    private readonly Mock<ISocket> socketMock = new(MockBehavior.Strict);
    private readonly SocketWrapper wrapper;

    public SocketWrapperTests()
    {
        wrapper = new(socketMock.Object);
    }

    [Fact]
    public async Task GetBytes_1()
    {
        socketMock.Setup(m => m.ReceiveAsync(It.IsAny<ArraySegment<byte>>())).Callback<ArraySegment<byte>>(buffer =>
        {
            Assert.Single(buffer);
            buffer[0] = 123;
        }).ReturnsAsync(1);
        Assert.Equal([123], await wrapper.GetBytes(1));
    }

    [Fact]
    public async Task GetBytes_10()
    {
        socketMock.Setup(m => m.ReceiveAsync(It.IsAny<ArraySegment<byte>>())).Callback<ArraySegment<byte>>(buffer =>
        {
            Assert.NotNull(buffer.Array);
            Assert.Equal(10, buffer.Array.Length);
            Array.Fill<byte>(buffer.Array, 123);
        }).ReturnsAsync(10);
        Assert.All(await wrapper.GetBytes(10), b => Assert.Equal(123, b));
    }

    [Theory]
    [MemberData(nameof(GetBytesData))]
    public async Task GetBytes(int[] lens)
    {
        int calls = 0;
        socketMock.Setup(m => m.ReceiveAsync(It.IsAny<ArraySegment<byte>>())).Callback<ArraySegment<byte>>(buffer =>
        {
            calls++;
            Assert.NotNull(buffer.Array);
            int len = lens[calls - 1];
            Assert.True(len <= buffer.Array.Length, $"{len} <= {buffer.Array.Length}");
            Array.Fill<byte>(buffer.Array, 123, 0, len);
        }).ReturnsAsync(() => lens[calls - 1]);
        int total = lens.Sum();
        byte[] bytes = await wrapper.GetBytes(total);
        Assert.Equal(lens.Length, calls);
        Assert.Equal(total, bytes.Length);
        Assert.All(bytes, b => Assert.Equal(123, b));
    }

    public static IEnumerable<object[]> GetBytesData =>
    [
        [new int[] { 2, 2 }],
        [new int[] { 2, 0, 2 }],
        [new int[] { 10, 10, 10, 10, 10, 10, 10 }],
        [new int[] { 1024, 1024 }],
    ];
}