using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parallelator.Client.Loaders;
using Parallelator.Common;
using Xunit;

namespace Parallelator.Client.Tests.Downloaders
{
    public abstract class LoaderTestBase<T>
    {
        private readonly int _delay;
        private readonly int _total;

        protected LoaderTestBase(int delay, int total)
        {
            _delay = delay;
            _total = total;
        }

        [Fact]
        public async Task DownloadAsync_ExpectCorrectNumOfResponses()
        {
            Uri[] uris = ApiUriBuilder.GenerateUris(_delay, _total);

            IThingyLoader<T> loader = CreateDownloader();
            T[] response = (await loader.LoadAsync(uris)).ToArray();

            Assert.Equal(_total, response.Distinct(CreateComparer()).Count());
        }

        protected abstract IThingyLoader<T> CreateDownloader();
        protected abstract IEqualityComparer<T> CreateComparer();
    }
}
