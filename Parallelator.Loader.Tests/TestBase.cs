using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parallelator.Common;
using Parallelator.Loaders;
using Xunit;

namespace Parallelator.Loader.Tests
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
            TThingy[] response;
            IThingyLoader<TThingy> loader = CreateLoader(ctorArgs);
            try
            {
                response = (await loader.LoadAsync(_uris)).ToArray();
            }
            finally
            {
                if (loader is IDisposable disposableLoader)
                {
                    disposableLoader.Dispose();
                }
            }

            Assert.Equal(_total, response.Distinct(comparer).Count());
            AssertResult(response);
        }

        public async Task TestExceptionPath(params object[] ctorArgs)
        {
            IThingyLoader<TThingy> loader = CreateLoader(ctorArgs);
            try
            {
                Func<Task> act = () => loader.LoadAsync(_uris);
                await Assert.ThrowsAnyAsync<Exception>(act);
            }
            finally
            {
                if (loader is IDisposable disposableLoader)
                {
                    disposableLoader.Dispose();
                }
            }
        }

        protected virtual void AssertResult(TThingy[] thingies)
        {
        }

        private static IThingyLoader<TThingy> CreateLoader(params object[] args) =>
            (IThingyLoader<TThingy>)Activator.CreateInstance(typeof(TLoader), args);
    }
}
