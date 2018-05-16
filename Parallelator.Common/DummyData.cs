using System.Diagnostics;

namespace Parallelator.Common
{
    [DebuggerDisplay("{Current} ({Total}, delay={Delay})")]
    public class DummyData
    {
        public DummyData(int delay, int total, int current)
        {
            Delay = delay;
            Total = total;
            Current = current;
        }

        public int Delay { get; }
        public int Total { get; }
        public int Current { get; }

        public DummyBallast Ballast { get; set; }
    }
}
