using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Client.Extensions;
using Parallelator.Common;

namespace Parallelator.Client.Loaders.Deserializing
{
    public class TaskSeqBatchDeserializingLoader : IThingyLoader<DummyData>
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        private readonly int _batchSize;

        public TaskSeqBatchDeserializingLoader(int batchSize)
        {
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            _batchSize = batchSize;
        }

        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            Uri[] input = uris as Uri[] ?? uris.ToArray();

            var result = new List<DummyData>();
            using (var client = new HttpClient())
            {
                // iterate over input data, await for batch of given size,
                // continue until all entries are not processed
                for (var i = 0; i < input.Length; i = i + _batchSize)
                {
                    IEnumerable<Task<DummyData>> tasks =
                        // ReSharper disable once AccessToDisposedClosure
                        input.Skip(i)
                            .Take(_batchSize)
                            .Select(async u => await client.GetPayloadAsync<DummyData>(u, Serializer));

                    // await all tasks in batch
                    result.AddRange(await Task.WhenAll(tasks));
                }
            }

            return result;
        }
    }
}
