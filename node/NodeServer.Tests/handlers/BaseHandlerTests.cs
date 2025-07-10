using Microsoft.Extensions.Logging;
using Moq;
using NodeServer.handlers;

namespace NodeServer.Tests.handlers;

public class BaseHandlerTests<T> : IDisposable where T : IRequestHandler
{
    protected readonly Mock<ILogger<T>> loggerMock = new();
    protected readonly Mock<IHandlerMap> mapMock = new();

    public BaseHandlerTests()
    {
        loggerMock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(false);
    }

    public virtual void Dispose()
    {
        loggerMock.VerifyNoOtherCalls();
        // should be in the ctor of any handler
        mapMock.Verify(m => m.AddHandler(It.IsAny<T>()));
        mapMock.VerifyNoOtherCalls();
        GC.SuppressFinalize(this);
    }
}