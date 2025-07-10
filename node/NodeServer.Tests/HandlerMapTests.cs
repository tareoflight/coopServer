using Moq;
using Node;
using NodeServer.handlers;

namespace NodeServer.Tests;

public sealed class HandlerMapTests : IDisposable
{
    public readonly HandlerMap requestMap = new();
    private readonly Mock<IRequestHandler> handlerMock = new();

    public HandlerMapTests()
    {
        handlerMock.Setup(m => m.RequestType).Returns(Request.RequestTypeOneofCase.None);
    }

    public void Dispose()
    {
        handlerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void AddHandler_Add()
    {
        requestMap.AddHandler(handlerMock.Object);
        handlerMock.VerifyGet(m => m.RequestType);
    }

    [Fact]
    public void AddHandler_Throw()
    {
        requestMap.AddHandler(handlerMock.Object);
        Assert.Throws<ArgumentException>(() => requestMap.AddHandler(handlerMock.Object));
        handlerMock.VerifyGet(m => m.RequestType);
    }

    [Fact]
    public void GetHandlerOrNull_Get()
    {
        requestMap.AddHandler(handlerMock.Object);
        handlerMock.VerifyGet(m => m.RequestType);
        IRequestHandler? handler = requestMap.GetHandlerOrNull(Request.RequestTypeOneofCase.None);
        Assert.Equal(handlerMock.Object, handler);
    }

    [Fact]
    public void GetHandlerOrNull_Null()
    {
        IRequestHandler? handler = requestMap.GetHandlerOrNull(Request.RequestTypeOneofCase.None);
        Assert.Null(handler);
    }
}