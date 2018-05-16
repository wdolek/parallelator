## Parallelator

Tiny miny benchmark of various data fetching (I/O) approaches. There's no exception handling, code contained in this
repository is __not production-ready__!

In order to run benchmark:

* Compile with `Release`
* Start `Parallelator.DummySite`
* Start `Parallelator.Client`

Have fun!

## Some results

``` ini

BenchmarkDotNet=v0.10.14, OS=Windows 10.0.17134
Intel Core i7-7600U CPU 2.80GHz (Kaby Lake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=2.1.200
  [Host]     : .NET Core 2.0.7 (CoreCLR 4.6.26328.01, CoreFX 4.6.26403.03), 64bit RyuJIT
  Job-LNDLLF : .NET Core 2.0.7 (CoreCLR 4.6.26328.01, CoreFX 4.6.26403.03), 64bit RyuJIT

LaunchCount=1  RunStrategy=Monitoring  TargetCount=3  
WarmupCount=0  

```
|                                        Method |    Categories | Total | Delay |      Mean |      Error |   StdDev | Scaled | ScaledSD | Rank |      Gen 0 |     Gen 1 | Allocated |
|---------------------------------------------- |-------------- |------ |------ |----------:|-----------:|---------:|-------:|---------:|-----:|-----------:|----------:|----------:|
|                               SequentialAsync |           Raw |   255 |   500 | 130.215 s |   5.4179 s | 0.3061 s |   1.00 |     0.00 |   ** |  5000.0000 | 1000.0000 |       0 B |
|                          SequentialBatchAsync |           Raw |   255 |   500 |   9.300 s |   0.8489 s | 0.0480 s |   0.07 |     0.00 |    * |  5000.0000 | 1000.0000 |   26880 B |
|                          ContinuousBatchAsync |           Raw |   255 |   500 |   9.287 s |   0.5123 s | 0.0289 s |   0.07 |     0.00 |    * |  5000.0000 | 1000.0000 |   23168 B |
|             SequentialBatchWithSemaphoreAsync |           Raw |   255 |   500 |   9.271 s |   0.3229 s | 0.0182 s |   0.07 |     0.00 |    * |  5000.0000 | 1000.0000 |  111200 B |
|                       DataFlowStrPayloadAsync |           Raw |   255 |   500 |   9.270 s |   0.9224 s | 0.0521 s |   0.07 |     0.00 |    * |  5000.0000 | 1000.0000 |   19288 B |
|                                               |               |       |       |           |            |          |        |          |      |            |           |           |
|                  SequentialDeserializingAsync | Deserializing |   255 |   500 | 130.468 s |   2.9817 s | 0.1685 s |   1.00 |     0.00 |   ** | 12000.0000 | 2000.0000 |       0 B |
|        ParallelInvokeDeserializingLoaderAsync | Deserializing |   255 |   500 |  13.731 s | 114.2119 s | 6.4532 s |   0.11 |     0.04 |    * |  9000.0000 | 2000.0000 |   57720 B |
|             ContinuousBatchDeserializingAsync | Deserializing |   255 |   500 |  10.699 s |  10.3554 s | 0.5851 s |   0.08 |     0.00 |    * | 10000.0000 | 2000.0000 |   26112 B |
|             SequentialBatchDeserializingAsync | Deserializing |   255 |   500 |   9.780 s |   9.0029 s | 0.5087 s |   0.07 |     0.00 |    * |  9000.0000 | 2000.0000 |   29824 B |
|             DataFlowDeserializingPayloadAsync | Deserializing |   255 |   500 |   9.416 s |   3.3517 s | 0.1894 s |   0.07 |     0.00 |    * |  8000.0000 | 2000.0000 |   22920 B |
| TaskEnumerableDeserializingWithSemaphoreAsync | Deserializing |   255 |   500 |   9.405 s |   3.3628 s | 0.1900 s |   0.07 |     0.00 |    * |  9000.0000 | 2000.0000 |  109240 B |
