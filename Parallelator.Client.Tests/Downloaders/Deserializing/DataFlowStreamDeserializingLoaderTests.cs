﻿using System.Threading.Tasks;
using Parallelator.Client.Loaders.Deserializing;
using Parallelator.Common;
using Xunit;

namespace Parallelator.Client.Tests.Downloaders.Deserializing
{
    public class DataFlowStreamDeserializingLoaderTests : TestBase<DataFlowStreamDeserializingLoader, DummyData>
    {
        public DataFlowStreamDeserializingLoaderTests() 
            : base(50, 100)
        {
        }

        [Fact]
        public async Task DownloadAsync_WhenLowConcurrency_ExpectCorretNumOfResults()
        {
            await TestHappyPath(DummyDataEqualityComparer.Instance, Constants.MaxConcurrency);
        }

        [Fact]
        public async Task DownloadAsync_WhenHighConcurrency_ExpectException()
        {
            await TestExceptionPath(Constants.MaxConcurrency + 1);
        }
    }
}