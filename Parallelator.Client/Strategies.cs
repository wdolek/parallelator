using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using Parallelator.Client.Loaders.Deserializing;
using Parallelator.Client.Loaders.Raw;
using Parallelator.Common;

namespace Parallelator.Client
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 1, warmupCount: 0, targetCount: 3)]
    [MemoryDiagnoser]
    [GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByCategory)]
    [CategoriesColumn]
    [RankColumn(NumeralSystem.Stars)]
    [OrderProvider(SummaryOrderPolicy.SlowestToFastest)]
    public class Strategies
    {
        private Uri[] _uris;

        [Params(1000)]
        public int NumOfEntries { get; set; }

        [Params(32,512)]
        public int ResponseDelay { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _uris = ApiUriBuilder.GenerateUris(ResponseDelay, NumOfEntries);
        }

        //[BenchmarkCategory("Sequential")]
        //[Benchmark]
        public async Task<IEnumerable<string>> SequentialAsync()
        {
            var downloader = new SequentialRawLoader();
            return await downloader.LoadAsync(_uris);
        }

        //[BenchmarkCategory("Sequential")]
        //[Benchmark]
        public async Task<IEnumerable<DummyData>> SequentialDeserializingAsync()
        {
            var downloader = new SequentialDeserializingLoader();
            return await downloader.LoadAsync(_uris);
        }

        [BenchmarkCategory("Raw")]
        [Benchmark]
        public async Task<IEnumerable<string>> ContinuousBatchAsync()
        {
            var downloader = new TaskContinuousBatchRawLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [BenchmarkCategory("Raw")]
        [Benchmark]
        public async Task<IEnumerable<string>> SequentialBatchAsync()
        {
            var downloader = new TaskSeqBatchRawLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [BenchmarkCategory("Raw")]
        [Benchmark]
        public async Task<IEnumerable<string>> SequentialBatchWithSemaphoreAsync()
        {
            var downloader = new TaskEnumWithSemaphoreRawLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [BenchmarkCategory("Raw")]
        [Benchmark]
        public async Task<IEnumerable<string>> DataFlowStrPayloadAsync()
        {
            var downloader = new DataFlowRawLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [BenchmarkCategory("Deserializing")]
        [Benchmark]
        public async Task<IEnumerable<DummyData>> ContinuousBatchDeserializingAsync()
        {
            var downloader = new TaskContinuousBatchDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [BenchmarkCategory("Deserializing")]
        [Benchmark]
        public async Task<IEnumerable<DummyData>> SequentialBatchDeserializingAsync()
        {
            var downloader = new TaskSeqBatchDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [BenchmarkCategory("Deserializing")]
        [Benchmark]
        public async Task<IEnumerable<DummyData>> TaskEnumWithSemaphoreDeserializingLoaderAsync()
        {
            var downloader = new TaskEnumWithSemaphoreDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [BenchmarkCategory("Deserializing")]
        [Benchmark]
        public async Task<IEnumerable<DummyData>> DataFlowDeserializingPayloadAsync()
        {
            var downloader = new DataFlowDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [BenchmarkCategory("Deserializing")]
        [Benchmark]
        public async Task<IEnumerable<DummyData>> ParallelInvokeDeserializingLoaderAsync()
        {
            var downloader = new ParallelInvokeDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }
    }
}
