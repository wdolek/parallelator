using Parallelator.Client.Downloaders;
using Parallelator.Client.Downloaders.Deserializing;
using Parallelator.Common;

namespace Parallelator.Client.Tests.Downloaders.Deserializing
{
    public class TaskEnumerableQueueDeserializingLoaderTests : DeserializingLoaderTestBase
    {
        public TaskEnumerableQueueDeserializingLoaderTests() 
            : base(10, 100)
        {
        }

        protected override IThingyLoader<DummyData> CreateDownloader() =>
            new TaskEnumerableQueueDeserializingLoader(Constants.MaxConcurrency);
    }
}
