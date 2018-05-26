using System.Threading.Tasks;
using Parallelator.Common;
using Parallelator.Loaders.Deserializing;
using Xunit;

namespace Parallelator.Loader.Tests.Deserializing
{
    public class Producer1Consumer1DeserializingLoaderTests : TestBase<Producer1Consumer1DeserializingLoader, DummyData>
    {
        public Producer1Consumer1DeserializingLoaderTests() 
            : base(50, 100)
        {
        }

        [Fact]
        public async Task DownloadAsync_WhenLowConcurrency_ExpectCorretNumOfResults()
        {
            await TestHappyPath(DummyDataEqualityComparer.Instance, Constants.MaxConcurrency);
        }
    }
}
