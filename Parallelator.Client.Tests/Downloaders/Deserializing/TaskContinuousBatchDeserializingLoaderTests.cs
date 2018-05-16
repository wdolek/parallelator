using Parallelator.Client.Loaders;
using Parallelator.Client.Loaders.Deserializing;
using Parallelator.Common;

namespace Parallelator.Client.Tests.Downloaders.Deserializing
{
    public class TaskContinuousBatchDeserializingLoaderTests : DeserializingLoaderTestBase
    {
        public TaskContinuousBatchDeserializingLoaderTests() 
            : base(10, 100)
        {
        }

        protected override IThingyLoader<DummyData> CreateDownloader() =>
            new TaskContinuousBatchDeserializingLoader(Constants.MaxConcurrency);
    }
}
