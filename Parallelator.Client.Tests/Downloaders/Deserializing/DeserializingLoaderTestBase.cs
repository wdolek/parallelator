using System.Collections.Generic;
using Parallelator.Common;

namespace Parallelator.Client.Tests.Downloaders.Deserializing
{
    public abstract class DeserializingLoaderTestBase : LoaderTestBase<DummyData>
    {
        protected DeserializingLoaderTestBase(int delay, int total)
            : base(delay, total)
        {
        }

        protected override IEqualityComparer<DummyData> CreateComparer() =>
            DummyDataEqualityComparer.Instance;
    }
}
