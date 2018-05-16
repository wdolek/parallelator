using Parallelator.Client.Downloaders;
using Parallelator.Client.Downloaders.Raw;
using Parallelator.Common;

namespace Parallelator.Client.Tests.Downloaders.Raw
{
    public class TaskContinuousBatchRawLoaderTests : RawLoaderTestBase
    {
        public TaskContinuousBatchRawLoaderTests()
            : base(10, 100)
        {
        }

        protected override IThingyLoader<string> CreateDownloader() =>
            new TaskContinuousBatchRawLoader(Constants.MaxConcurrency);
    }
}
