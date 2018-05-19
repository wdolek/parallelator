using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Columns;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using Parallelator.Client.Loaders.Deserializing;
using Parallelator.Common;

namespace Parallelator.Client.Benchmarks
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 1, warmupCount: 0, targetCount: 3)]
    [MemoryDiagnoser]
    [RankColumn(NumeralSystem.Stars)]
    [OrderProvider(SummaryOrderPolicy.SlowestToFastest)]
    public class DeserializingLoaderBenchmarks
    {
        private Uri[] _uris;

        [Params(1000)]
        public int NumOfEntries { get; set; }

        [Params(512)]
        public int ResponseDelay { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            _uris = ApiUriBuilder.GenerateUris(ResponseDelay, NumOfEntries);
        }

        //[Benchmark]
        public async Task<IEnumerable<DummyData>> SequentialDeserializingAsync()
        {
            var downloader = new SequentialDeserializingLoader();
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<DummyData>> ContinuousBatchDeserializingAsync()
        {
            var downloader = new TaskContinuousBatchDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<DummyData>> SequentialBatchDeserializingAsync()
        {
            var downloader = new TaskSeqBatchDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<DummyData>> TaskEnumWithSemaphoreDeserializingLoaderAsync()
        {
            var downloader = new TaskEnumWithSemaphoreDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<DummyData>> DataFlowDeserializingPayloadAsync()
        {
            var downloader = new DataFlowDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<DummyData>> DataFlowStreamDeserializingLoaderAsync()
        {
            var downloader = new DataFlowStreamDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<DummyData>> ParallelInvokeDeserializingLoaderAsync()
        {
            var downloader = new ParallelInvokeDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<DummyData>> ParallelForEachDeserializingLoaderAsync()
        {
            var downloader = new ParallelForEachDeserializingLoader(Constants.MaxConcurrency);
            return await downloader.LoadAsync(_uris);
        }

        [Benchmark]
        public async Task<IEnumerable<DummyData>> ProducerConsumerDeserializingLoaderAsync()
        {
            using (var downloader = new ProducerConsumerDeserializingLoader(Constants.MaxConcurrency))
            {
                return await downloader.LoadAsync(_uris);
            }
        }
    }
}
