using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using Parallelator.Common;

namespace Parallelator.Client.Downloaders.Deserializing
{
    // https://stackoverflow.com/questions/22492383/throttling-asynchronous-tasks/22492731#22492731
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

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            IList<DummyData> result;
            using (var client = new HttpClient())
            {
                var downloader = new TransformBlock<Uri, string>(
                    async u => await client.GetStringAsync(u),
                    new ExecutionDataflowBlockOptions
                    {
                        MaxDegreeOfParallelism = _maxParallelism,
                        EnsureOrdered = false,
                        SingleProducerConstrained = true
                    });

                var deserializer =
                    new TransformBlock<string, DummyData>(
                        s => JsonConvert.DeserializeObject<DummyData>(s),
                        new ExecutionDataflowBlockOptions
                        {
                            MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
                            EnsureOrdered = false,
                            SingleProducerConstrained = true
                        });

                var buffer = new BufferBlock<DummyData>(
                    new ExecutionDataflowBlockOptions
                    {
                        EnsureOrdered = false
                    });

                var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
                downloader.LinkTo(deserializer, linkOptions);
                deserializer.LinkTo(buffer, linkOptions);

                foreach (Uri uri in uris)
                {
                    await downloader.SendAsync(uri);
                }

                downloader.Complete();
                await deserializer.Completion;

                buffer.TryReceiveAll(out result);
            }

            return result;
        }
    }
}
