using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

Console.WriteLine("Starting node server");

IPAddress addr = IPAddress.Parse("127.0.0.1");
int port = 25569;
IPEndPoint endpoint = new(addr, port);

using Socket socket = new(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
Console.WriteLine("binding...");
socket.Bind(endpoint);
socket.Listen(100);

Socket handler = await socket.AcceptAsync();



while (true)
{
    List<byte[]> chunks = [];
    byte[] sizeBuffer = new byte[4];
    int received = await handler.ReceiveAsync(sizeBuffer, SocketFlags.None);
    do
    {
        byte[] buffer = new byte[1_024];
        int recieved = await handler.ReceiveAsync(buffer, SocketFlags.None);
    } while (true);
}
