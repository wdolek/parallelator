using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Client.Extensions;
using Parallelator.Common;

namespace Parallelator.Client.Downloaders.Deserializing
{
    public class SequentialDeserializingLoader : IThingyLoader<DummyData>
    {
        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            if (uris == null)
            {
                throw new ArgumentNullException(nameof(uris));
            }

            var result = new LinkedList<DummyData>();
            using (var client = new HttpClient())
            {
                foreach (Uri uri in uris)
                {
                    result.AddLast(await client.GetPayloadAsync<DummyData>(uri, Serializer));
                }
            }

            return result;
        }
    }
}
