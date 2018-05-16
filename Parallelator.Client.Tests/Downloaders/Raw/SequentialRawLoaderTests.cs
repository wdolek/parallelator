using Parallelator.Client.Downloaders;
using Parallelator.Client.Downloaders.Raw;

namespace Parallelator.Client.Tests.Downloaders.Raw
{
    public class SequentialRawLoaderTests : RawLoaderTestBase
    {
        public SequentialRawLoaderTests() 
            : base(10, 100)
        {
        }

        protected override IThingyLoader<string> CreateDownloader() =>
            new SequentialRawLoader();
    }
}
