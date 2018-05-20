using System.Threading.Tasks;
using Parallelator.Common;
using Parallelator.Loaders.Deserializing;
using Xunit;

namespace Parallelator.Loader.Tests.Deserializing
{
    public class ProducerConsumerDeserializingLoaderTests : TestBase<ProducerConsumerDeserializingLoader, DummyData>
    {
        public ProducerConsumerDeserializingLoaderTests() 
            : base(50, 100)
        {
        }

        [Fact]
        public async Task DownloadAsync_WhenLowConcurrency_ExpectCorretNumOfResults()
        {
            await TestHappyPath(DummyDataEqualityComparer.Instance, Constants.MaxConcurrency);
        }

        [Fact]
        public async Task DownloadAsync_WhenHighConcurrency_ExpectException()
        {
            await TestExceptionPath(Constants.MaxConcurrency * 2);
        }
    }
}
