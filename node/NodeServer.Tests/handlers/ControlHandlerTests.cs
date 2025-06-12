using Microsoft.Extensions.Logging;
using Node;
using NodeServer.handlers;

namespace NodeServer.Tests.handlers;

public sealed class ControlHandlerTests : BaseHandlerTests
{
    private readonly ControlHandler handler;

    public ControlHandlerTests()
    {
        handler = new(factoryMock.Object);
    }

    [Fact]
    public async Task Handle_Unknown()
    {
        Request request = new() { Control = new ControlRequest() };
        await handler.Handle(request);
        loggerMock.Verify(m => m.IsEnabled(LogLevel.Debug));
        loggerMock.Verify(m => m.IsEnabled(LogLevel.Warning));
    }

    [Fact]
    public async Task Handle_Shutdown()
    {
        Request request = new() { Control = new ControlRequest() { Shutdown = new Shutdown() } };
        bool didFire = false;
        handler.Shutdown += (s, e) => didFire = true;
        await handler.Handle(request);
        Assert.True(didFire, "Shutdown event did not fire");
        loggerMock.Verify(m => m.IsEnabled(LogLevel.Debug));
    }

    [Fact]
    public void RequestType()
    {
        Assert.Equal(Request.RequestTypeOneofCase.Control, handler.RequestType);
    }
}