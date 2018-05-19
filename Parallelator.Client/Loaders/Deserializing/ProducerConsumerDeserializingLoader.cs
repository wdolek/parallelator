using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Parallelator.Common;

namespace Parallelator.Client.Loaders.Deserializing
{
    /// <summary>
    /// Implementing simple producer-consumer pattern. Since producer gives us only `Task` which are created quickly
    /// throttling is achieved by combination of blocking collection capacity and speed of consumer.
    /// This requires some manual fiddling to find sweet spot. For better control, semaphore could be use, but that
    /// is approach for another loader.
    /// </summary>
    public sealed class ProducerConsumerDeserializingLoader : IThingyLoader<DummyData>, IDisposable
    {
        private readonly HttpClient _client;
        private readonly int _boundedCapacity;

        public ProducerConsumerDeserializingLoader(int boundedCapacity)
        {
            if (boundedCapacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(boundedCapacity));
            }

            _client = new HttpClient();

            // since cosnumer only fires request and consumer awaits it afterwards,
            // we must lower bounded capacity to not start too many requests (creating task is fast)
            _boundedCapacity = Math.Max(1, boundedCapacity / 2);
        }

        public async Task<IEnumerable<DummyData>> LoadAsync(IEnumerable<Uri> uris)
        {
            var bag = new ConcurrentBag<DummyData>();
            var workingQueue = new BlockingCollection<Task<string>>(_boundedCapacity);

            /**
             * if bounded capacity = 2, then:
             *
             * 1 2 3 4 5 6 7 8 9 0 1
             * ---------------------
             * P P P - P - - - - - -
             * - C . C . C . C . C .
             *
             * 1:
             *  producer: start 1st request (capacity left = 1)
             *  consumer: idle
             * 2:
             *  producer: start 2nd request (capacity left = 0)
             *  consumer: await 1st request (capacity left = 1)
             * 3:
             *  producer: start 3rd request (capacity left = 0)
             *  consumer: deserialize 1st request
             * 4:
             *  producer: idle
             *  consumer: await 2nd request (capacity left = 1)
             * 5:
             *  producer: start 4th request (capacity left = 0)
             *  consumer: deserialize 2nd request
             * ...
             */

            // start producer: fires HTTP request for each URI
            Task producer = Task.Factory.StartNew(
                () =>
                {
                    foreach (Uri uri in uris)
                    {
                        // blocking when queue reached its bounded capacity
                        // NB! we collect only `Task`, so this part is fast
                        workingQueue.Add(_client.GetStringAsync(uri));
                    }

                    workingQueue.CompleteAdding();
                });

            // start consumer: reads request tasks from collection and deserializes response
            Task consumer = Task.Factory.StartNew(
                () =>
                {
                    while (!workingQueue.IsCompleted)
                    {
                        try
                        {
                            // blocking here (!) - waiting for request and its content (.Result)
                            Task<string> task = workingQueue.Take();
                            string response = task.Result;

                            bag.Add(JsonConvert.DeserializeObject<DummyData>(response));
                        }
                        catch (InvalidOperationException)
                        {
                            break;
                        }
                    }
                });

            // await consumer only (won't end before producer, so we don't really care about it)
            await consumer;

            return bag;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
