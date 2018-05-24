using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Common;

namespace Parallelator.Loaders.Deserializing
{
    /// <summary>
    /// Producer-consumer using N1 threads as loading workers and N2 threads deserializing workers.
    /// </summary>
    public sealed class ProducerConsumerThreadsDeserializingLoader : IThingyLoader<DummyData>, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly int _workerCount;

        private readonly ConcurrentQueue<Uri> _sourceQueue;
        private readonly ConcurrentQueue<string> _serializationQueue;

        private readonly ConcurrentBag<DummyData> _bag;

        private int _runningLoaders;
        private Exception _exception;

        public ProducerConsumerThreadsDeserializingLoader(int workerCount)
        {
            if (workerCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(workerCount));
            }

            _httpClient = new HttpClient();
            _workerCount = workerCount;

            _sourceQueue = new ConcurrentQueue<Uri>();
            _serializationQueue = new ConcurrentQueue<string>();

            _bag = new ConcurrentBag<DummyData>();
        }

        /// <inheritdoc />
        public Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            // enqueue all URIs (should be fast)
            foreach (Uri uri in uris)
            {
                _sourceQueue.Enqueue(uri);
            }

            // queue is ready, start threading
            var loadingWorkers = new Thread[_workerCount];
            for (var i = 0; i < loadingWorkers.Length; i++)
            {
                loadingWorkers[i] = new Thread(LoadPayload);
                loadingWorkers[i].Start();
            }

            var deserializationWorkers = new Thread[Environment.ProcessorCount];
            for (var i = 0; i < deserializationWorkers.Length; i++)
            {
                deserializationWorkers[i] = new Thread(DeserializePayload);
                deserializationWorkers[i].Start();
            }

            // "wait" for all threads to finish
            // (this blocks calling thread)
            foreach (Thread deserializationThread in deserializationWorkers)
            {
                deserializationThread.Join();
            }

            if (_exception != null)
            {
                ExceptionDispatchInfo.Capture(_exception).Throw();
            }

            return Task.FromResult((IEnumerable<DummyData>)_bag);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        private void LoadPayload()
        {
            Interlocked.Increment(ref _runningLoaders);
            while (true)
            {
                // queue has been populated before we started loading
                // - if we can't dequeue, we reached end
                if (!_sourceQueue.TryDequeue(out Uri uri))
                {
                    break;
                }

                try
                {
                    string payload = _httpClient.GetStringAsync(uri).Result;
                    _serializationQueue.Enqueue(payload);
                }
                catch (Exception e)
                {
                    _exception = e;
                    Interlocked.Exchange(ref _runningLoaders, 0);

                    return;
                }
            }

            int numOfLoaders = Interlocked.Decrement(ref _runningLoaders);

            // not atomic operation, but at least last exiting thread will leave correct value
            if (numOfLoaders < 0)
            {
                Interlocked.Exchange(ref _runningLoaders, 0);
            }
        }

        private void DeserializePayload()
        {
            while (_runningLoaders > 0)
            {
                if (!_serializationQueue.TryDequeue(out string payload))
                {
                    continue;
                }

                var dummy = JsonConvert.DeserializeObject<DummyData>(payload);
                _bag.Add(dummy);
            }
        }
    }
}
