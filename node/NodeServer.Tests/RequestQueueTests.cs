using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Node;

namespace NodeServer.Tests;

public sealed class RequestQueueTests : IDisposable
{
    private readonly CancellationTokenSource cancel = new();
    private readonly RequestQueue requestQueue;
    private readonly Mock<IRequestDispatcher> dispatcher = new();

    public RequestQueueTests()
    {
        requestQueue = new RequestQueue(NullLogger.Instance, dispatcher.Object);
    }

    public void Dispose()
    {
        dispatcher.VerifyNoOtherCalls();
    }

    [Fact]
    public async void DequeueLoop_Cancels()
    {
        Task task = requestQueue.DequeueLoop(cancel.Token);
        await cancel.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
    }

    [Fact]
    public async Task DequeueLoop_PreCancel()
    {
        await cancel.CancelAsync();
        await requestQueue.DequeueLoop(cancel.Token);
        // no exception means it didn't get into the waits
    }

    [Fact]
    public async void DequeueLoop_Dispatch()
    {
        Task task = requestQueue.DequeueLoop(cancel.Token);
        Request request = new();
        await requestQueue.EnqueueRequest(request, cancel.Token);
        dispatcher.Verify(d => d.Dispatch(request));
        await cancel.CancelAsync();
        await Assert.ThrowsAsync<OperationCanceledException>(async () => await task);
    }
}