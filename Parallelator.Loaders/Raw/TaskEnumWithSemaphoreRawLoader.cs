using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Parallelator.Loaders.Raw
{
    // https://stackoverflow.com/questions/22492383/throttling-asynchronous-tasks/22493662#22493662
    public class TaskEnumWithSemaphoreRawLoader : IThingyLoader<string>
    {
        private readonly int _batchSize;

        public TaskEnumWithSemaphoreRawLoader(int batchSize)
        {
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            _batchSize = batchSize;
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task<IEnumerable<string>> LoadAsync(IEnumerable<Uri> uris)
        {
            using (var semaphore = new SemaphoreSlim(_batchSize))
            using (var httpClient = new HttpClient())
            {
                IEnumerable<Task<string>> tasks = uris.Select(
                    async u =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            return await httpClient.GetStringAsync(u);
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });

                return await Task.WhenAll(tasks);
            }
        }
    }
}
