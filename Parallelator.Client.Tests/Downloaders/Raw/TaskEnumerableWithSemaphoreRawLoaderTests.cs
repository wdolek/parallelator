using Parallelator.Client.Downloaders;
using Parallelator.Client.Downloaders.Raw;
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
            new TaskEnumerableWithSemaphoreRawLoader(Constants.MaxConcurrency);
    }
}
