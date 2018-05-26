using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parallelator.Loaders.Extensions
{
    public static class EnumerableExtensions
    {
        // https://blogs.msdn.microsoft.com/pfxteam/2012/03/05/implementing-a-simple-foreachasync-part-2/
        public static Task ForEachAsync<T>(this IEnumerable<T> source, int dop, Func<T, Task> body) =>
            Task.WhenAll(
                Partitioner.Create(source)
                    .GetPartitions(dop)
                    .Select(
                        partition => Task.Run(
                            async delegate
                            {
                                using (partition)
                                {
                                    while (partition.MoveNext())
                                    {
                                        await body(partition.Current);
                                    }
                                }
                            })));
    }
}
