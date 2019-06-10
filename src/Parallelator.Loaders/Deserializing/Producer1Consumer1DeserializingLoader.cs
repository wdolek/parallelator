using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Common;

namespace Parallelator.Loaders.Deserializing
{
    /// <summary>
    /// Sequential procuder-consumer implementation - over-engineering at its best!
    /// </summary>
    public sealed class Producer1Consumer1DeserializingLoader : IThingyLoader<DummyData>, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly int _boundedCapacity;

        public Producer1Consumer1DeserializingLoader(int boundedCapacity)
        {
            if (boundedCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(boundedCapacity));
            }

            _httpClient = new HttpClient();
            _boundedCapacity = boundedCapacity;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            // load payload (blocking, .Result)
            var payloads = new BlockingCollection<string>(_boundedCapacity);
            Task loader = Task.Factory.StartNew(
                () =>
                {
                    foreach (Uri uri in uris)
                    {
                        payloads.Add(_httpClient.GetStringAsync(uri).Result);
                    }

                    payloads.CompleteAdding();
                });

            // deserialize payloads (happens sequentially again, se we don't need concurrent collection)
            var result = new List<DummyData>();
            Task deserializer = Task.Factory.StartNew(
                () =>
                {
                    while (!payloads.IsCompleted)
                    {
                        string payload = payloads.Take();
                        result.Add(JsonConvert.DeserializeObject<DummyData>(payload));
                    }
                });

            await deserializer;

            return result;
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
