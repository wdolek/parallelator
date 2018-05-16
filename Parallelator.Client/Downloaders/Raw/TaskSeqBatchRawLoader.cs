using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parallelator.Client.Downloaders.Raw
{
    public class TaskSeqBatchRawLoader : IThingyLoader<string>
    {
        private readonly int _batchSize;

        public TaskSeqBatchRawLoader(int batchSize)
        {
            if (batchSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSize));
            }

            _batchSize = batchSize;
        }

        public async Task<IEnumerable<string>> LoadAsync(IEnumerable<Uri> uris)
        {
            // prevent possible repetitive enumeration && make access easier
            Uri[] input = uris as Uri[] ?? uris.ToArray();

            var result = new List<string>();
            using (var client = new HttpClient())
            {
                for (var i = 0; i < input.Length; i = i + _batchSize)
                {
                    IEnumerable<Task<string>> tasks =
                        // ReSharper disable once AccessToDisposedClosure
                        input.Skip(i)
                            .Take(_batchSize)
                            .Select(async u => await client.GetStringAsync(u));

                    result.AddRange(await Task.WhenAll(tasks));
                }
            }

            return result;
        }
    }
}
