﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Parallelator.Client.Loaders.Raw
{
    // https://stackoverflow.com/questions/22492383/throttling-asynchronous-tasks/22492731#22492731
    public class DataFlowRawLoader : IThingyLoader<string>
    {
        private readonly int _maxParallelism;

        public DataFlowRawLoader(int maxParallelism)
        {
            if (maxParallelism < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxParallelism));
            }

            _maxParallelism = maxParallelism;
        }

        [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
        public async Task<IEnumerable<string>> LoadAsync(IEnumerable<Uri> uris)
        {
            IList<string> result;
            using (var client = new HttpClient())
            {
                var buffer = new BufferBlock<string>();
                var downloader = new TransformBlock<Uri, string>(
                    async u => await client.GetStringAsync(u),
                    new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = _maxParallelism });

                downloader.LinkTo(buffer);

                foreach (Uri uri in uris)
                {
                    await downloader.SendAsync(uri);
                }

                downloader.Complete();
                await downloader.Completion;

                buffer.TryReceiveAll(out result);
            }

            return result;
        }
    }
}