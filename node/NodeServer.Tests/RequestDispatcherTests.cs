namespace NodeServer.Tests;

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Node;
using NodeServer;
using NodeServer.handlers;

public sealed class RequestDispatcherTests : IDisposable
{
    private readonly RequestDispatcher dispatcher;
    private readonly Mock<ILogger> loggerMock = new();
    private readonly Mock<IRequestHandler> handlerMock = new();

    public RequestDispatcherTests()
    {
        dispatcher = new(loggerMock.Object);
        handlerMock.Setup(m => m.RequestType).Returns(Request.RequestTypeOneofCase.None);
    }

    public void Dispose()
    {
        loggerMock.VerifyNoOtherCalls();
        handlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void AddHandler_Add()
    {
        dispatcher.AddHandler(handlerMock.Object);
        handlerMock.VerifyGet(m => m.RequestType);
    }

    [Fact]
    public void AddHandler_Throw()
    {
        dispatcher.AddHandler(handlerMock.Object);
        Assert.Throws<ArgumentException>(() => dispatcher.AddHandler(handlerMock.Object));
        handlerMock.VerifyGet(m => m.RequestType);
    }

    [Fact]
    public async Task Dispatch_Handler()
    {
        Request request = new(); // None type
        dispatcher.AddHandler(handlerMock.Object);
        await dispatcher.Dispatch(request);
        handlerMock.VerifyGet(m => m.RequestType);
        handlerMock.Verify(m => m.Handle(request));
    }

    [Fact]
    public async Task Dispatch_Unknown()
    {
        Request request = new();
        loggerMock.Setup(m => m.IsEnabled(LogLevel.Warning)).Returns(true);
        await dispatcher.Dispatch(request);
        loggerMock.Verify(m => m.IsEnabled(LogLevel.Warning));
        loggerMock.Verify(m => m.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()));
    }
}