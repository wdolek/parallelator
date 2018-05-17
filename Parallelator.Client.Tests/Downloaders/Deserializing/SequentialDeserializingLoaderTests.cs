using System.Threading.Tasks;
using Parallelator.Client.Loaders.Deserializing;
using Parallelator.Common;
using Xunit;

namespace Parallelator.Client.Tests.Downloaders.Deserializing
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
