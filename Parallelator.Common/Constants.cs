using System;

namespace Parallelator.Common
{
    public static class Constants
    {
        public static readonly int MaxConcurrency = Environment.ProcessorCount * 2;
        public static readonly string FeedScheme = "http";
        public static readonly string FeedHost = "localhost";
        public static readonly int FeedPort = 5000;
        public static readonly string FeedBasePath = "/api";
        public static readonly string CliDisableMaxConcurrency = "--boundless";
    }
}
