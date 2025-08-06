namespace NodeServer.Tests.stream;

public class SlowStream : Stream
{
    protected static async Task Slow(CancellationToken token)
    {
        try
        {
            await Task.Delay(1000, token);
        }
        catch (TaskCanceledException)
        {
            throw new OperationCanceledException();
        }
    }

    // the only functions we actually care about
    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await Slow(cancellationToken);
        throw new Exception("Wasn't Canceled!");
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        await Slow(cancellationToken);
        throw new Exception("Wasn't Canceled!");
    }

    // the rest is just abstract class boilerplate

    public override bool CanRead => throw new NotImplementedException();

    public override bool CanSeek => throw new NotImplementedException();

    public override bool CanWrite => throw new NotImplementedException();

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }
}