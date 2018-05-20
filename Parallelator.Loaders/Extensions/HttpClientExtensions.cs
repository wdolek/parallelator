using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Parallelator.Loaders.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetPayloadAsync<T>(this HttpClient client, Uri uri, JsonSerializer serializer)
            where T : class
        {
            using (Stream s = await client.GetStreamAsync(uri))
            using (var sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return serializer.Deserialize<T>(reader);
            }
        }
    }
}
