using Node;

namespace NodeServer;

public class RequestReader(Stream stream)
{
    private readonly StreamWrapper wrapper = new(stream);

    public async Task<Request> GetRequestAsync()
    {
        int size = await wrapper.ReadSizeAsync();
        return Request.Parser.ParseFrom(await wrapper.GetBytesAsync(size));
    }
}