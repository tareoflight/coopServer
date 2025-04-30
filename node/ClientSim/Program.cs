using System.Net;
using System.Net.Sockets;
using System.Text;
using Google.Protobuf;
using Node;

Console.WriteLine("Starting node client");

IPAddress addr = IPAddress.Parse("127.0.0.1");
int port = 25569;
IPEndPoint endpoint = new(addr, port);

using Socket socket = new(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

await socket.ConnectAsync(endpoint);
using NetworkStream stream = new(socket);
using BinaryWriter writer = new(stream);
uint requestId = 1;
bool loop = true;
while (loop)
{
    W("==ClientSim==");
    W("1...Control");
    string selection = R("Choose a request to send");
    Request request = new()
    {
        RequestId = requestId++,
    };
    switch (selection)
    {
        case "1":
            ControlRequest control = new()
            {
                Shutdown = new ()
            };
            request.Control = control;
            byte[] bytes = request.ToByteArray();
            writer.Write(bytes.Length);
            writer.Write(bytes);
            Console.WriteLine(bytes.Length);
            Console.WriteLine(string.Join(" ", bytes.Select(b => b.ToString())));
            break;
        case "q":
            W("Exiting...");
            loop = false;
            break;
    }
}

socket.Shutdown(SocketShutdown.Both);

void W(string line)
{
    Console.WriteLine(line);
}

string R(string prompt, string? def = null)
{
    Console.Write(prompt);
    if (def != null)
    {
        Console.Write($" [{def}]");
    }
    Console.Write(": ");
    while (true)
    {
        string? line = Console.ReadLine();
        if (line == null)
        {
            if (def != null)
            {
                return def;
            }
        }
        else
        {
            return line.Trim();
        }
    }
}
