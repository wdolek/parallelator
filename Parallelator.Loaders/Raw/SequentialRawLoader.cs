using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parallelator.Loaders.Raw
{
    public class SequentialRawLoader : IThingyLoader<string>
    {
        public async Task<IEnumerable<string>> LoadAsync(IEnumerable<Uri> uris)
        {
            Uri[] input = uris as Uri[] ?? uris.ToArray();

            var result = new List<string>(input.Length);
            using (var client = new HttpClient())
            {
                foreach (Uri uri in input)
                {
                    result.Add(await client.GetStringAsync(uri));
                }
            }

            return result;
        }
    }
}
