using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using Parallelator.Common;
using Parallelator.Loaders.Raw;

namespace Parallelator.Benchmark.Benchmarks
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 1, warmupCount: 0, targetCount: 3)]
    [MemoryDiagnoser]
    [RankColumn(NumeralSystem.Stars)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class RawLoaderBenchmarks
    {
        private readonly Consumer _consumer = new Consumer();

        private Uri[] _uris;

        [Params(255)]
        public int NumOfEntries { get; set; }

        [Params(512)]
        public int ResponseDelay { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _uris = ApiUriBuilder.GenerateUris(ResponseDelay, NumOfEntries);
        }

        [Benchmark]
        public async Task SequentialRawLoaderAsync()
        {
            var downloader = new SequentialRawLoader();
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark]
        public async Task TaskContinuousBatchRawLoaderAsync()
        {
            var downloader = new TaskContinuousBatchRawLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark]
        public async Task TaskSeqBatchRawLoaderAsync()
        {
            var downloader = new TaskSeqBatchRawLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark]
        public async Task TaskEnumWithSemaphoreRawLoaderAsync()
        {
            var downloader = new TaskEnumWithSemaphoreRawLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark]
        public async Task DataFlowRawLoaderAsync()
        {
            var downloader = new DataFlowRawLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }
    }
}
