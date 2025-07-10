using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Node;

Console.WriteLine("Starting node client");

IPAddress addr = IPAddress.Parse("127.0.0.1");
int port = 25569;
IPEndPoint endpoint = new(addr, port);

using Socket socket = new(addr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

while (true)
{
    try
    {
        W("Connecting to server...");
        await socket.ConnectAsync(endpoint);
        break;
    }
    catch (SocketException)
    {
        W("Server not ready, retrying...");
        await Task.Delay(1000);
    }
}

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
                Shutdown = new()
            };
            request.ControlRequest = control;
            byte[] bytes = request.ToByteArray();
            writer.Write(bytes.Length);
            writer.Write(bytes);
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
