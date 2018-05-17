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
                IEnumerable<Task> tasks = input.Select(async u =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        string payload = await httpClient.GetStringAsync(u);
                        processing.Enqueue(payload);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                // await all download tasks
                await Task.WhenAll(tasks);

                // await deserialization
                // - at this point, we know that we collected all data for deserialization
                //   (for general usage we would need to lock queue/processing to avoid race condition)
                return await processing.Completion;
            }
        }
       
        /// <summary>
        /// Processing consumer.
        /// </summary>
        /// <remarks>
        /// This consumer is not thread safe! Implemented minimalist way to demonstrate downloading/deserialization.
        /// </remarks>
        /// <typeparam name="TThingy">Type of thingy.</typeparam>
        private class Processing<TThingy>
            where TThingy : class
        {
            private readonly List<Task<TThingy>> _pending;
            private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(Environment.ProcessorCount);

            public Processing(int queueSize)
            {
                _pending = new List<Task<TThingy>>(queueSize);
            }

            public Task<TThingy[]> Completion => Task.WhenAll(_pending);

            public void Enqueue(string data) => 
                _pending.Add(ProcessAsync(data));

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
