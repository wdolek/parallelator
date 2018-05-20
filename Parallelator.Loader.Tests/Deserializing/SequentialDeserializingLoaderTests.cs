using System.Threading.Tasks;
using Parallelator.Common;
using Parallelator.Loaders.Deserializing;
using Xunit;

namespace Parallelator.Loader.Tests.Deserializing
{
    public class SequentialDeserializingLoaderTests : TestBase<SequentialDeserializingLoader, DummyData>
    {
        public SequentialDeserializingLoaderTests()
            : base(10, 100)
        {
        }

        [Fact]
        public async Task DownloadAsync_WhenLowConcurrency_ExpectCorretNumOfResults()
        {
            await TestHappyPath(DummyDataEqualityComparer.Instance);
        }
    }
}
