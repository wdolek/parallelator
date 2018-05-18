using BenchmarkDotNet.Running;
using Parallelator.Client.Benchmarks;

namespace Parallelator.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[] {
                typeof(RawLoaderBenchmarks),
                typeof(DeserializingLoaderBenchmarks)
            });
            switcher.Run(args);
        }
    }
}