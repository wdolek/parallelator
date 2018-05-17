using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parallelator.Client.Loaders;
using Parallelator.Common;
using Xunit;

namespace Parallelator.Client.Tests.Downloaders
{
    public abstract class TestBase<TLoader, TThingy>
        where TLoader : IThingyLoader<TThingy>
    {
        private readonly int _total;
        private readonly Uri[] _uris;

        protected TestBase(int delay, int total)
        {
            _total = total;
            _uris = ApiUriBuilder.GenerateUris(delay, total);
        }

        public async Task TestHappyPath(IEqualityComparer<TThingy> comparer, params object[] ctorArgs)
        {
            IThingyLoader<TThingy> loader = CreateLoader(ctorArgs);
            TThingy[] response = (await loader.LoadAsync(_uris)).ToArray();

            Assert.Equal(_total, response.Distinct(comparer).Count());
        }

        public async Task TestExceptionPath(params object[] ctorArgs)
        {
            IThingyLoader<TThingy> loader = CreateLoader(ctorArgs);
            Func<Task> act = () => loader.LoadAsync(_uris);

            await Assert.ThrowsAnyAsync<Exception>(act);
        }

        private static IThingyLoader<TThingy> CreateLoader(params object[] args) =>
            (IThingyLoader<TThingy>)Activator.CreateInstance(typeof(TLoader), args);
    }
}
