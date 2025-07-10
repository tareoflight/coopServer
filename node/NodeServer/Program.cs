namespace NodeServer;

using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NodeServer.handlers;

public partial class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddActivatedSingleton<ControlHandler>();
        builder.Services.AddSingleton<IRequestQueue, RequestQueue>();
        builder.Services.AddSingleton<IHandlerMap, HandlerMap>();

        builder.Services.AddHostedService<RequestDispatcher>();
        builder.Services.AddHostedService<ReceiveService>();

        using IHost host = builder.Build();
        await host.RunAsync();
    }
}