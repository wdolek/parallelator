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

namespace Parallelator.Client.Loaders.Deserializing
{
    /// <summary>
    /// Thingy loader using <see cref="Parallel.Invoke(ParallelOptions,Action[])"/>.
    /// Action which runs in parallel takes care of downloading and parsing at once. Loading thread is blocked
    /// by calling <see cref="Task{T}.Result"/>.
    /// </summary>
    /// <remarks><see href="http://blog.danskingdom.com/tag/c-task-thread-throttle-limit-maximum-simultaneous-concurrent-parallel/"/></remarks>
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

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            // data collection allowing multi-threaded access
            var bag = new ConcurrentBag<DummyData>();

            using (var client = new HttpClient())
            {
                // build array of actions to run in parallel
                Action[] actions = uris.Select<Uri, Action>(
                        u => () => { bag.Add(client.GetPayloadAsync<DummyData>(u, Serializer).Result); })
                    .ToArray();

                // invoke with options
                Parallel.Invoke(
                    new ParallelOptions
                    {
                        MaxDegreeOfParallelism = _maxParallelism
                    },
                    actions);
            }

            return Task.FromResult<IEnumerable<DummyData>>(bag);
        }
    }
}
