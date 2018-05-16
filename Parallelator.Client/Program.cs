using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Parallelator.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            Summary summary = BenchmarkRunner.Run<Strategies>();
        }
    }
}