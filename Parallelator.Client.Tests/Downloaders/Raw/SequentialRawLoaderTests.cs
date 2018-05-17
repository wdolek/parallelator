using System.Collections.Generic;
using System.Threading.Tasks;
using Parallelator.Client.Loaders.Raw;
using Xunit;

namespace Parallelator.Client.Tests.Downloaders.Raw
{
    public class SequentialRawLoaderTests : TestBase<SequentialRawLoader, string>
    {
        public SequentialRawLoaderTests() 
            : base(10, 100)
        {
        }

        [Fact]
        public async Task DownloadAsync_WhenLowConcurrency_ExpectCorretNumOfResults()
        {
            await TestHappyPath(EqualityComparer<string>.Default);
        }
    }
}
