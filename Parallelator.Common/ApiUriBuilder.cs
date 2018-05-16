using System;
using System.Linq;

namespace Parallelator.Common
{
    public static class ApiUriBuilder
    {
        private static readonly Uri BaseUri = new Uri("http://localhost:5000/api/dummy/", UriKind.Absolute);

        public static Uri[] GenerateUris(int delay, int total) =>
            Enumerable.Range(0, total)
                .Select(i => new Uri(BaseUri, $"{delay}/{total}/{i}"))
                .ToArray();
    }
}
