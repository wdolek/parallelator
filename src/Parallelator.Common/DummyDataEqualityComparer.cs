using System.Collections.Generic;

namespace Parallelator.Common
{
    public class DummyDataEqualityComparer : IEqualityComparer<DummyData>
    {
        public static readonly IEqualityComparer<DummyData> Instance = new DummyDataEqualityComparer();

        public bool Equals(DummyData x, DummyData y) =>
            ReferenceEquals(x, y) || x?.Current == y?.Current;

        public int GetHashCode(DummyData obj) =>
            obj?.GetHashCode() ?? -1;
    }
}
