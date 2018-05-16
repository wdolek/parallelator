using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Common;

namespace Parallelator.Client.Loaders.Deserializing
{
    // https://stackoverflow.com/questions/22492383/throttling-asynchronous-tasks/22493662#22493662
    // (slightly modified version processing deserialization)
    public class TaskEnumerableQueueDeserializingLoader : IThingyLoader<DummyData>
    {
        private readonly int _maxParallelism;

        public TaskEnumerableQueueDeserializingLoader(int maxParallelism)
        {
            if (maxParallelism < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxParallelism));
            }

            _maxParallelism = maxParallelism;
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            var processing = new Processing<DummyData>();

            using (var httpClient = new HttpClient())
            using (var semaphore = new SemaphoreSlim(_maxParallelism))
            {
                IEnumerable<Task> tasks = uris.Select(async u =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        string payload = await httpClient.GetStringAsync(u);
                        processing.QueueItem(payload);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                await Task.WhenAll(tasks.ToArray());

                return await processing.WaitForCompleteAsync();
            }
        }
       
        private class Processing<TThingy>
            where TThingy : class
        {
            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(Environment.ProcessorCount);
            private readonly HashSet<Task<TThingy>> _pending = new HashSet<Task<TThingy>>();

            private readonly object _lock = new object();

            public void QueueItem(string data)
            {
                Task<TThingy> task = ProcessAsync(data);
                lock (_lock)
                {
                    _pending.Add(task);
                }
            }

            public async Task<TThingy[]> WaitForCompleteAsync()
            {
                Task<TThingy>[] tasks;
                lock (_lock)
                {
                    tasks = _pending.ToArray();
                    _pending.Clear();
                }

                return await Task.WhenAll(tasks);
            }

            private async Task<TThingy> ProcessAsync(string data)
            {
                await _semaphore.WaitAsync();
                try
                {
                    return JsonConvert.DeserializeObject<TThingy>(data);
                }
                finally
                {
                    _semaphore.Release();
                }
            }
        }
    }
}
