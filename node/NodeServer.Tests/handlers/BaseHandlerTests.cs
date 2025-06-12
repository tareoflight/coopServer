using Microsoft.Extensions.Logging;
using Moq;

namespace NodeServer.Tests.handlers;

public class BaseHandlerTests : IDisposable
{
    protected readonly Mock<ILogger> loggerMock = new();
    protected readonly Mock<ILoggerFactory> factoryMock = new();

    public BaseHandlerTests()
    {
        loggerMock.Setup(m => m.IsEnabled(It.IsAny<LogLevel>())).Returns(false);
        factoryMock.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(loggerMock.Object);
    }

    public virtual void Dispose()
    {
        loggerMock.VerifyNoOtherCalls();
        factoryMock.Verify(m => m.CreateLogger(It.IsAny<string>()));
        factoryMock.VerifyNoOtherCalls();
    }
}