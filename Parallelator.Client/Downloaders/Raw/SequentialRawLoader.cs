using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Parallelator.Client.Downloaders.Raw
{
    public class SequentialRawLoader : IThingyLoader<string>
    {
        public async Task<IEnumerable<string>> LoadAsync(IEnumerable<Uri> uris)
        {
            if (uris == null)
            {
                throw new ArgumentNullException(nameof(uris));
            }

            var result = new LinkedList<string>();
            using (var client = new HttpClient())
            {
                foreach (Uri uri in uris)
                {
                    result.AddLast(await client.GetStringAsync(uri));
                }
            }

            return result;
        }
    }
}
