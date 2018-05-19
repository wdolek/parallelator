## Parallelator

Tiny miny benchmark of various data fetching (I/O) strategies. There's no exception handling, code contained in this
repository is __not production-ready__!

In order to run benchmark:

* Compile with `Release`
* Run `run-api.bat`
* Run `run-benchmark.bat`
* Wait...

Have fun!

## Some results

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.1.200
  [Host]     : .NET Core 2.0.7 (CoreCLR 4.6.26328.01, CoreFX 4.6.26403.03), 64bit RyuJIT
  Job-EJKLJA : .NET Core 2.0.7 (CoreCLR 4.6.26328.01, CoreFX 4.6.26403.03), 64bit RyuJIT

LaunchCount=1  RunStrategy=Monitoring  TargetCount=3  
WarmupCount=0  

```
|                                       Method | NumOfEntries | ResponseDelay |      Mean |     Error |    StdDev | Rank |      Gen 0 |     Gen 1 | Allocated |
|--------------------------------------------- |------------- |-------------- |----------:|----------:|----------:|-----:|-----------:|----------:|----------:|
|                         &#39;Loading one by one&#39; |          255 |           512 | 136.704 s |   5.804 s |  0.3280 s |   ** | 12000.0000 | 2000.0000 |       0 B |
| &#39;Producer-Consumer with blocking collection&#39; |          255 |           512 |  23.673 s | 266.425 s | 15.0535 s |    * |  9000.0000 | 2000.0000 |       0 B |
|                   &#39;Parallel.Invoke with DoP&#39; |          255 |           512 |  13.448 s |  93.124 s |  5.2617 s |    * |  9000.0000 | 2000.0000 |   53136 B |
|           &#39;ForEachAsync with concurrent bag&#39; |          255 |           512 |  11.661 s |   6.758 s |  0.3818 s |    * | 10000.0000 | 2000.0000 |    9488 B |
|         &#39;Awaiting batches (groups) of tasks&#39; |          255 |           512 |  11.408 s |  25.304 s |  1.4297 s |    * |  8000.0000 | 2000.0000 |   29824 B |
|        &#39;Always keeping n tasks concurrently&#39; |          255 |           512 |  10.676 s |  10.465 s |  0.5913 s |    * | 10000.0000 | 2000.0000 |   28136 B |
|                 &#39;Data-Flow obtaining stream&#39; |          255 |           512 |   9.884 s |   3.211 s |  0.1814 s |    * |  8000.0000 | 2000.0000 |   22920 B |
|                 &#39;Data-Flow obtaining string&#39; |          255 |           512 |   9.856 s |   4.170 s |  0.2356 s |    * |  9000.0000 | 2000.0000 |   22920 B |
|           &#39;Producer-Consumer with semaphore&#39; |          255 |           512 |   9.838 s |   2.749 s |  0.1553 s |    * |  9000.0000 | 2000.0000 |  113488 B |
