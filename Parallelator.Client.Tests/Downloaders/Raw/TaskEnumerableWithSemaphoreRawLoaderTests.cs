using Parallelator.Client.Loaders;
using Parallelator.Client.Loaders.Raw;
using Parallelator.Common;

namespace Parallelator.Client.Tests.Downloaders.Raw
{
    public class TaskEnumerableWithSemaphoreRawLoaderTests : RawLoaderTestBase
    {
        public TaskEnumerableWithSemaphoreRawLoaderTests()
            : base(10, 100)
        {
        }

        protected override IThingyLoader<string> CreateDownloader() =>
            new TaskEnumWithSemaphoreRawLoader(Constants.MaxConcurrency);
    }
}
