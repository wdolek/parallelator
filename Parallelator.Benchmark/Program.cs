using BenchmarkDotNet.Running;
using Parallelator.Benchmark.Benchmarks;

namespace Parallelator.Benchmark
{
    public class Program
    {
        static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[] {
                typeof(DeserializingLoaderBenchmarks),
                typeof(RawLoaderBenchmarks),
            });
            switcher.Run(args);
        }
    }
}