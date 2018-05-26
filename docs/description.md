## Thingy loaders

There are two main categories of loaders. First category called "Raw" is just downloading payload
as string, which is then returned to caller. Second category called "Deserializing" is also
deserializing payload to provided type.

Loading of payload is I/O bound operation, whereas deserialization is just CPU bound. Some loaders
are trying to make use of this by splitting parallelization according to type of workload.

- [Raw loaders](raw_loaders.md): take payload and keep it as string
- [Deserializing loaders](deserializing_loaders.md): load payload and deserialize it

