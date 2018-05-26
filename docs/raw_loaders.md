## Sequential loader

[SequentialRawLoader.cs](../src/Parallelator.Loaders/Raw/SequentialRawLoader.cs) is the most straightforward
implementation of loader. Content is loaded sequentially - asynchronously, awaiting every request.

## Splitting and awaiting whole batch

[TaskSeqBatchRawLoader.cs](../src/Parallelator.Loaders/Raw/TaskSeqBatchRawLoader.cs) simply splits input
enumerable to batches and then simply awaiting whole batch by awaiting `Task.WhenAll(batch)`

This approach takes time of slowest request, before getting to next batch. This way we may end up
with delays between batches.

## Continuous batch processing

[TaskContinuousBatchRawLoader.cs](../src/Parallelator.Loaders/Raw/TaskContinuousBatchRawLoader.cs) is trying
to improve approach of previous loader by keeping batch full whole time.

Whenever one task is done, we process it (store payload) and fire new loading task. Loading is slightly
improved but if multiple requests finishes at same time, we process them sequentially.

## Enumeration with semaphore

[TaskEnumWithSemaphoreRawLoader.cs](../src/Parallelator.Loaders/Raw/TaskEnumWithSemaphoreRawLoader.cs) uses
`SemaphoreSlim` to throttle load. Similarly as previous loader, "batch" is continous, always having
as much items possible.

Main trick is here that `Task.WhenAll(tasks)` awaits enumerable which is generated way that only
given number of tasks are active.

Concept taken from [SO: Throttling asynchronous tasks](https://stackoverflow.com/questions/22492383/throttling-asynchronous-tasks/22493662#22493662).

## TPL Dataflow

[DataFlowRawLoader.cs](../src/Parallelator.Loaders/Raw/DataFlowRawLoader.cs) - as name suggests, this
loaded uses [TPL Dataflow](https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/dataflow-task-parallel-library)
framework to implement producer-consumer pipeline. With Dataflow, it is simple to implement
loading which runs with certain degree of parallelism.