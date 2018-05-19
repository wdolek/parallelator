using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Client.Extensions;
using Parallelator.Common;

namespace Parallelator.Client.Loaders.Deserializing
{
    /// <summary>
    /// Using <see cref="EnumerableExtensions.ForEachAsync{T}"/> with specified degree of parallelism (dop).
    /// </summary>
    public class ParallelForEachDeserializingLoader : IThingyLoader<DummyData>
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        private readonly int _dop;

        public ParallelForEachDeserializingLoader(int dop)
        {
            if (dop < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(dop));
            }

            _dop = dop;
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            var bag = new ConcurrentBag<DummyData>();
            using (var client = new HttpClient())
            {
                await uris.ForEachAsync(
                    _dop,
                    async u => { bag.Add(await client.GetPayloadAsync<DummyData>(u, Serializer)); });
            }

            return bag;
        }
    }
}
