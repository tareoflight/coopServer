using Node;

namespace NodeServer;

public interface IRequestDispatcher
{
    public Task Dispatch(Request request);
}