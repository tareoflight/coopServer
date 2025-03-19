using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Starting node server");

IPAddress addr = IPAddress.Parse("127.0.0.1");
int port = 25569;
IPEndPoint endpoint = new(addr, port);

using Socket socket = new(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
Console.WriteLine("binding...");
socket.Bind(endpoint);
socket.Listen(100);

Socket handler = await socket.AcceptAsync();
const string eom = "<|EOM|>";
const string ack = "<|ACK|>";

while (true)
{
    byte[] buffer = new byte[1_024];
    int recieved = await handler.ReceiveAsync(buffer, SocketFlags.None);
    string response = Encoding.UTF8.GetString(buffer, 0, recieved);

    if (response.Contains(eom))
    {
        Console.WriteLine($"Socket server received message: \"{response.Replace(eom, "")}\"");
        byte[] ackBytes = Encoding.UTF8.GetBytes(ack);
        await handler.SendAsync(ackBytes, 0);

        Console.WriteLine($"Socket server sent acknowledgment: \"{ack}\"");
    }
}