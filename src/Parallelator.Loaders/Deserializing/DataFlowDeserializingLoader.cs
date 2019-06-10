using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using Parallelator.Common;

namespace Parallelator.Loaders.Deserializing
{
    /// <summary>
    /// Thingy loader which uses TPL data flow.
    /// Pipeline consists of:
    /// - transformation URL -> string (actual loading)
    /// - string -> object (deserializing)
    /// - buffer (for receiving processed entries)
    /// </summary>
    /// <remarks><see href="https://stackoverflow.com/questions/22492383/throttling-asynchronous-tasks/22492731#22492731"/></remarks>
    public class DataFlowDeserializingLoader : IThingyLoader<DummyData>
    {
        private readonly int _maxParallelism;

        public DataFlowDeserializingLoader(int maxParallelism)
        {
            if (maxParallelism < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxParallelism));
            }

            _maxParallelism = maxParallelism;
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            IList<DummyData> result;
            using (var client = new HttpClient())
            {
                // downloader block with parallelism limit
                var downloader = new TransformBlock<Uri, string>(
                    async u => await client.GetStringAsync(u),
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = _maxParallelism,
                        EnsureOrdered = false,
                        SingleProducerConstrained = true
                    });

                // deserializer, unbound parallelism
                // (since we throttle producer (downloader), it won't probably spin more tasks anyway)
                var deserializer =
                    new TransformBlock<string, DummyData>(
                        s => JsonConvert.DeserializeObject<DummyData>(s),
                        new ExecutionDataflowBlockOptions
                        {
                            MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
                            EnsureOrdered = false,
                            SingleProducerConstrained = true
                        });

                // buffer to access result
                var buffer = new BufferBlock<DummyData>(
                    new ExecutionDataflowBlockOptions
                    {
                        EnsureOrdered = false
                    });

                // link blocks together
                var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
                downloader.LinkTo(deserializer, linkOptions);
                deserializer.LinkTo(buffer, linkOptions);

                // start sending input
                foreach (Uri uri in uris)
                {
                    await downloader.SendAsync(uri);
                }

                // flag input completed
                downloader.Complete();

                // await whole pipeline, get result
                await deserializer.Completion;
                buffer.TryReceiveAll(out result);
            }

            return result;
        }
    }
}
