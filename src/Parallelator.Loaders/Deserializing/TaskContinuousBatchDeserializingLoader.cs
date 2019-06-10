using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Common;
using Parallelator.Loaders.Extensions;

namespace Parallelator.Loaders.Deserializing
{
    /// <summary>
    /// Loader which tries to keep maximum tasks running in parallel implementing unsorted "queue".
    /// </summary>
    public class TaskContinuousBatchDeserializingLoader : IThingyLoader<DummyData>
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        private readonly int _batchSize;

        public TaskContinuousBatchDeserializingLoader(int batchSize)
        {
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            _batchSize = batchSize;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            Uri[] input = uris as Uri[] ?? uris.ToArray();

            var queue = new List<Task<DummyData>>(_batchSize);
            var result = new List<DummyData>(input.Length);

            using (var client = new HttpClient())
            {
                foreach (Uri uri in input)
                {
                    // enqueue downloading & deserializing task
                    queue.Add(client.GetPayloadAsync<DummyData>(uri, Serializer));

                    // if queue reached its size, await one task (dequeue) and continue
                    if (queue.Count == _batchSize)
                    {
                        Task<DummyData> finishedTask = await Task.WhenAny(queue);
                        queue.Remove(finishedTask);
                        result.Add(finishedTask.Result);
                    }
                }

                // process rest of the queue
                if (queue.Count > 0)
                {
                    DummyData[] finishedTaskResults = await Task.WhenAll(queue);
                    foreach (DummyData taskResult in finishedTaskResults)
                    {
                        result.Add(taskResult);
                    }
                }
            }

            return result;
        }
    }
}
