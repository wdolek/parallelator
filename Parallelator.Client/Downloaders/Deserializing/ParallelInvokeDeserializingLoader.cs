using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Client.Extensions;
using Parallelator.Common;

namespace Parallelator.Client.Downloaders.Deserializing
{
    // http://blog.danskingdom.com/tag/c-task-thread-throttle-limit-maximum-simultaneous-concurrent-parallel/
    public class ParallelInvokeDeserializingLoader : IThingyLoader<DummyData>
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        private readonly int _maxParallelism;

        public ParallelInvokeDeserializingLoader(int maxParallelism)
        {
            if (maxParallelism < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxParallelism));
            }

            _maxParallelism = maxParallelism;
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            var bag = new ConcurrentBag<DummyData>();
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = _maxParallelism
            };

            using (var client = new HttpClient())
            {
                Action[] actions = uris.Select<Uri, Action>(
                        u => () => { bag.Add(client.GetPayloadAsync<DummyData>(u, Serializer).Result); })
                    .ToArray();

                Parallel.Invoke(options, actions);
            }

            return Task.FromResult<IEnumerable<DummyData>>(bag);
        }
    }
}
