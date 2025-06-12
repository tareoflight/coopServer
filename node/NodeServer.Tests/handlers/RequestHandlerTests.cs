using Microsoft.Extensions.Logging;
using Node;
using NodeServer.handlers;

namespace NodeServer.Tests.handlers;

public sealed class RequestHandlerTests : BaseHandlerTests
{
    private class TestRequestHandler(ILoggerFactory loggerFactory) : RequestHandler(loggerFactory)
    {
        public override Request.RequestTypeOneofCase RequestType => throw new NotImplementedException();

        public override Task Handle(Request request) => throw new NotImplementedException();

        // expose the logger
        public ILogger Logger => logger;
    }

    private readonly TestRequestHandler handler;

    public RequestHandlerTests()
    {
        handler = new(factoryMock.Object);
    }

    public override void Dispose()
    {
        factoryMock.Verify(m => m.CreateLogger("NodeServer.Tests.handlers.RequestHandlerTests.TestRequestHandler"));
        base.Dispose();
    }

    [Fact]
    public void LoggerCreated()
    {
        Assert.Equal(loggerMock.Object, handler.Logger);
    }
}
