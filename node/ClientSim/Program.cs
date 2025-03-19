using System.Net;
using System.Net.Sockets;
using System.Text;

Console.WriteLine("Starting node client");

IPAddress addr = IPAddress.Parse("127.0.0.1");
int port = 25569;
IPEndPoint endpoint = new(addr, port);

using Socket socket = new(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

await socket.ConnectAsync(endpoint);
while (true)
{
    string message = "Hi friends 👋!<|EOM|>";
    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
    await socket.SendAsync(messageBytes, SocketFlags.None);
    Console.WriteLine($"Socket client sent message: \"{message}\"");

    byte[] buffer = new byte[1_024];
    int received = await socket.ReceiveAsync(buffer, SocketFlags.None);
    string response = Encoding.UTF8.GetString(buffer, 0, received);
    if (response == "<|ACK|>") {
        Console.WriteLine($"Socket Client received acknowledgement: \"{response}\"");
        break;
    }
}

socket.Shutdown(SocketShutdown.Both);