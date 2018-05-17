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
    /// <summary>
    /// Sequential loader.
    /// </summary>
    public class SequentialDeserializingLoader : IThingyLoader<DummyData>
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            Uri[] input = uris as Uri[] ?? uris.ToArray();

            var result = new List<DummyData>(input.Length);
            using (var client = new HttpClient())
            {
                foreach (Uri uri in input)
                {
                    // download and deserialize
                    result.Add(await client.GetPayloadAsync<DummyData>(uri, Serializer));
                }
            }

            return result;
        }
    }
}
