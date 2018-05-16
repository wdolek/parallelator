using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parallelator.Client.Loaders.Raw
{
    public class TaskContinuousBatchRawLoader : IThingyLoader<string>
    {
        private readonly int _batchSize;

        public TaskContinuousBatchRawLoader(int batchSize)
        {
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            _batchSize = batchSize;
        }

        public async Task<IEnumerable<string>> LoadAsync(IEnumerable<Uri> uris)
        {
            var result = new LinkedList<string>();
            var queue = new List<Task<string>>(_batchSize);

            using (var client = new HttpClient())
            using (IEnumerator<Uri> enumerator = uris.GetEnumerator())
            {
                while (true)
                {
                    bool hasNext = enumerator.MoveNext();

                    // we have reached end, process rest of queue if not empty and break
                    if (!hasNext)
                    {
                        if (queue.Count > 0)
                        {
                            string[] finishedTaskResults = await Task.WhenAll(queue);
                            foreach (string taskResult in finishedTaskResults)
                            {
                                result.AddLast(taskResult);
                            }
                        }

                        break;
                    }

                    // enqueue new loading task
                    Uri uri = enumerator.Current;
                    queue.Add(client.GetStringAsync(uri));

                    // we have reached point when queue is full, await one task and continue
                    if (queue.Count == _batchSize)
                    {
                        Task<string> finishedTask = await Task.WhenAny(queue);
                        queue.Remove(finishedTask);
                        result.AddLast(finishedTask.Result);
                    }
                }
            }

            return result;
        }
    }
}
