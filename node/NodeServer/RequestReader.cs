using Node;

namespace NodeServer;

public class RequestReader(Stream stream)
{
    private readonly StreamWrapper wrapper = new(stream);

    public async Task<Request> GetRequestAsync(CancellationToken token)
    {
        int size = await wrapper.ReadSizeAsync(token);
        return Request.Parser.ParseFrom(await wrapper.GetBytesAsync(size, token));
    }
}