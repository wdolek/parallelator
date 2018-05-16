using System;

namespace Parallelator.Common
{
    public static class Constants
    {
        public static readonly int MaxConcurrency = Environment.ProcessorCount * 4;
    }
}
