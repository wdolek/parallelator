using Parallelator.Client.Loaders;
using Parallelator.Client.Loaders.Deserializing;
using Parallelator.Common;

namespace Parallelator.Client.Tests.Downloaders.Deserializing
{
    public class ParallelInvokeDeserializingLoaderTests : DeserializingLoaderTestBase
    {
        public ParallelInvokeDeserializingLoaderTests()
            : base(10, 100)
        {
        }

        protected override IThingyLoader<DummyData> CreateDownloader() =>
            new ParallelInvokeDeserializingLoader(Constants.MaxConcurrency);
    }
}
