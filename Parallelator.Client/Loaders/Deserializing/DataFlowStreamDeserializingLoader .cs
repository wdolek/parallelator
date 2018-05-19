using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using Parallelator.Common;

namespace Parallelator.Client.Loaders.Deserializing
{
    /// <summary>
    /// Thingy loader which uses TPL data flow, deserializing from stream.
    /// </summary>
    public class DataFlowStreamDeserializingLoader : IThingyLoader<DummyData>
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        private readonly int _maxParallelism;

        public DataFlowStreamDeserializingLoader(int maxParallelism)
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
                var downloader = new TransformBlock<Uri, HttpResponseMessage>(
                    async u => await client.GetAsync(u),
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = _maxParallelism,
                        EnsureOrdered = false,
                        SingleProducerConstrained = true
                    });

                // deserializer, unbound parallelism
                var deserializer =
                    new TransformBlock<HttpResponseMessage, DummyData>(
                        async r =>
                        {
                            using (Stream s = await r.Content.ReadAsStreamAsync())
                            using (var sr = new StreamReader(s))
                            using (JsonReader reader = new JsonTextReader(sr))
                            {
                                return Serializer.Deserialize<DummyData>(reader);
                            }
                        },
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

                // input completed
                downloader.Complete();

                // await deserializer
                await deserializer.Completion;

                // pipeline done, receive result
                buffer.TryReceiveAll(out result);
            }

            return result;
        }
    }
}
