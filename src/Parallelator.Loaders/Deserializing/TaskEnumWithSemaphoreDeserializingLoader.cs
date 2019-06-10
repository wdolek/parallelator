using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Common;

namespace Parallelator.Loaders.Deserializing
{
    /// <summary>
    /// Throttling enumeration of downloading and later deserialization tasks using <see cref="SemaphoreSlim"/>.
    /// </summary>
    /// <remarks><see href="https://stackoverflow.com/questions/22492383/throttling-asynchronous-tasks/22493662#22493662"/></remarks>
    public class TaskEnumWithSemaphoreDeserializingLoader : IThingyLoader<DummyData>
    {
        private readonly int _maxParallelism;

        public TaskEnumWithSemaphoreDeserializingLoader(int maxParallelism)
        {
            if (maxParallelism < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxParallelism));
            }

            _maxParallelism = maxParallelism;
        }

        /// <inheritdoc />
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            Uri[] input = uris as Uri[] ?? uris.ToArray();

            var processing = new Processing<DummyData>(input.Length);

            using (var httpClient = new HttpClient())
            using (var semaphore = new SemaphoreSlim(_maxParallelism))
            {
                // build up enumerable of downloading tasks with throttling using semaphore
                // - if we reach critical number of tasks, next iteration awaits semaphore to open from previous task
                // - when loading is complete, result is enqueued to processing consumer which handles deserialization
                // - enqueing of tasks is sequential
                // - at the end `tasks` will contain completed tasks representing download
                IEnumerable<Task> tasks = input.Select(
                    async u =>
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

                await Task.WhenAll(tasks);

                // await deserialization
                // - at this point, we know that we collected all data for deserialization
                //   (for general usage we would need to lock queue/processing to avoid race condition)
                return await processing.WaitForCompleteAsync();
            }
        }

        private class Processing<TThingy>
            where TThingy : class
        {
            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(Environment.ProcessorCount);
            private readonly List<Task<TThingy>> _pending;

            private readonly object _lock = new object();

            public Processing(int queueSize)
            {
                _pending = new List<Task<TThingy>>(queueSize);
            }

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
