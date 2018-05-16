using System.Collections.Generic;

namespace Parallelator.Client.Tests.Downloaders.Raw
{
    public abstract class RawLoaderTestBase : LoaderTestBase<string>
    {
        protected RawLoaderTestBase(int delay, int total)
            : base(delay, total)
        {
        }

        protected override IEqualityComparer<string> CreateComparer() =>
            EqualityComparer<string>.Default;
    }
}
