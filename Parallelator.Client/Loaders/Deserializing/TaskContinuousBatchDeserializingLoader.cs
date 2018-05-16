using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Client.Extensions;
using Parallelator.Common;

namespace Parallelator.Client.Loaders.Deserializing
{
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

        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            var result = new LinkedList<DummyData>();
            var queue = new List<Task<DummyData>>(_batchSize);

            using (var client = new HttpClient())
            using (IEnumerator<Uri> enumerator = uris.GetEnumerator())
            {
                while (true)
                {
                    bool hasNext = enumerator.MoveNext();
                    if (!hasNext)
                    {
                        if (queue.Count > 0)
                        {
                            DummyData[] finishedTaskResults = await Task.WhenAll(queue);
                            foreach (DummyData taskResult in finishedTaskResults)
                            {
                                result.AddLast(taskResult);
                            }
                        }

                        break;
                    }

                    Uri uri = enumerator.Current;
                    queue.Add(client.GetPayloadAsync<DummyData>(uri, Serializer));

                    if (queue.Count == _batchSize)
                    {
                        Task<DummyData> finishedTask = await Task.WhenAny(queue);
                        queue.Remove(finishedTask);
                        result.AddLast(finishedTask.Result);
                    }
                }
            }

            return result;
        }
    }
}
