using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using Parallelator.Client.Loaders.Raw;
using Parallelator.Common;

namespace Parallelator.Client.Benchmarks
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 1, warmupCount: 0, targetCount: 3)]
    [MemoryDiagnoser]
    [RankColumn(NumeralSystem.Stars)]
    [OrderProvider(SummaryOrderPolicy.SlowestToFastest)]
    public class RawLoaderBenchmarks
    {
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
        public async Task<IEnumerable<string>> SequentialRawLoaderAsync()
        {
            var downloader = new SequentialRawLoader();
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<string>> TaskContinuousBatchRawLoaderAsync()
        {
            var downloader = new TaskContinuousBatchRawLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<string>> TaskSeqBatchRawLoaderAsync()
        {
            var downloader = new TaskSeqBatchRawLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<string>> TaskEnumWithSemaphoreRawLoaderAsync()
        {
            var downloader = new TaskEnumWithSemaphoreRawLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<string>> DataFlowRawLoaderAsync()
        {
            var downloader = new DataFlowRawLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }
    }
}
