using Parallelator.Client.Downloaders;
using Parallelator.Client.Downloaders.Deserializing;
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
