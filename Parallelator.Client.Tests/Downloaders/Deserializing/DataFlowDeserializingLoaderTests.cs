using Parallelator.Client.Loaders;
using Parallelator.Client.Loaders.Deserializing;
using Parallelator.Common;

namespace Parallelator.Client.Tests.Downloaders.Deserializing
{
    public class DataFlowDeserializingLoaderTests : DeserializingLoaderTestBase
    {
        public DataFlowDeserializingLoaderTests() 
            : base(50, 1000)
        {
        }

        protected override IThingyLoader<DummyData> CreateDownloader() =>
            new DataFlowDeserializingLoader(Constants.MaxConcurrency);
    }
}
