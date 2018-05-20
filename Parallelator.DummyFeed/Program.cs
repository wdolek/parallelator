using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Parallelator.DummyFeed
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel(o =>
                {
                    o.Limits.MaxConcurrentConnections = Common.Constants.MaxConcurrency;
                    o.Listen(IPAddress.Any, 5000);
                })
                .UseStartup<Startup>()
                .Build();
    }
}
