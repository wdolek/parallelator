using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Parallelator.Common;

namespace Parallelator.DummyFeed
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args)
            .UseKestrel(o => ConfigureKestrel(o, args))
            .UseStartup<Startup>()
            .Build();

        private static KestrelServerOptions ConfigureKestrel(KestrelServerOptions o, string[] args)
        {
            // if args contain `--boundless true`, don't configure max concurrency
            int boundlessSwitchIdx = Array.FindIndex(
                args,
                s => string.Equals(s, Constants.CliDisableMaxConcurrency, StringComparison.OrdinalIgnoreCase));

            bool isBoundless = boundlessSwitchIdx >= 0
                               && boundlessSwitchIdx + 1 < args.Length
                               && string.Equals(
                                   args[boundlessSwitchIdx + 1],
                                   "true",
                                   StringComparison.OrdinalIgnoreCase);

            // configure
            o.Listen(IPAddress.Any, Constants.FeedPort);
            if (!isBoundless)
            {
                o.Limits.MaxConcurrentConnections = Constants.MaxConcurrency;
            }

            return o;
        }
    }
}
