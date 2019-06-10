using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using Parallelator.Common;
using Parallelator.Loaders.Deserializing;

namespace Parallelator.Benchmark.Benchmarks
{
    [SimpleJob(RunStrategy.Monitoring, launchCount: 1, warmupCount: 0, targetCount: 3)]
    [MemoryDiagnoser]
    [RankColumn(NumeralSystem.Stars)]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class DeserializingLoaderBenchmarks
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

        [Benchmark(Description = "Loading one by one")]
        public async Task SequentialDeserializingAsync()
        {
            var downloader = new SequentialDeserializingLoader();
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark(Description = "Always keeping n tasks concurrently")]
        public async Task TaskContinuousBatchDeserializingLoaderAsync()
        {
            var downloader = new TaskContinuousBatchDeserializingLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark(Description = "Awaiting batches (groups) of tasks")]
        public async Task TaskSeqBatchDeserializingLoaderAsync()
        {
            var downloader = new TaskSeqBatchDeserializingLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark(Description = "Producer-Consumer with semaphore")]
        public async Task TaskEnumWithSemaphoreDeserializingLoaderLoaderAsync()
        {
            var downloader = new TaskEnumWithSemaphoreDeserializingLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark(Description = "TPL Dataflow obtaining string")]
        public async Task DataFlowDeserializingLoaderAsync()
        {
            var downloader = new DataFlowDeserializingLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark(Description = "TPL Dataflow obtaining stream")]
        public async Task DataFlowStreamDeserializingLoaderAsync()
        {
            var downloader = new DataFlowStreamDeserializingLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark(Description = "Parallel.Invoke with degree of parallelism")]
        public async Task ParallelInvokeDeserializingLoaderAsync()
        {
            var downloader = new ParallelInvokeDeserializingLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark(Description = "ForEachAsync with concurrent bag as storage")]
        public async Task ParallelForEachDeserializingLoaderAsync()
        {
            var downloader = new ParallelForEachDeserializingLoader(Constants.MaxConcurrency);
            (await downloader.LoadAsync(_uris)).Consume(_consumer);
        }

        [Benchmark(Description = "Producer-Consumer 1:1, producer->task, consumer->await & deserializes")]
        public async Task ProducerTaskConsumerAwaitsDeserializingLoaderAsync()
        {
            using (var downloader = new ProducerTaskConsumerAwaitsDeserializingLoader(Constants.MaxConcurrency))
            {
                (await downloader.LoadAsync(_uris)).Consume(_consumer);
            }
        }

        [Benchmark(Description = "Producer-Consumer 1:1, producer->string, consumer->deserializes")]
        public async Task Producer1Consumer1DeserializingLoaderAsync()
        {
            using (var downloader = new Producer1Consumer1DeserializingLoader(Constants.MaxConcurrency))
            {
                (await downloader.LoadAsync(_uris)).Consume(_consumer);
            }
        }

        //[Benchmark(Description = "Producer-Consumer with Threads")]
        public async Task ProducerConsumerThreadsDeserializingLoaderAsync()
        {
            using (var downloader = new ProducerConsumerThreadsDeserializingLoader(Constants.MaxConcurrency))
            {
                (await downloader.LoadAsync(_uris)).Consume(_consumer);
            }
        }
    }
}
