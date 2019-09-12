using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace LibraryTests.Tests
{
    public class ListAddVsAddRange
    {
        [Params(100, 1000, 10000, 100000, 1000000)]
        public int Length { get; set; }

        private List<TestItem> DummyData { get; set; }

        [Benchmark(Description = "List<T> AddRange")]
        public void ListAddRangeTest()
        {
            var Data = new List<TestItem>();
            Data.AddRange(DummyData);
        }

        [Benchmark(Description = "List<T> Add", Baseline = true)]
        public void ListTest()
        {
            var Data = new List<TestItem>();
            for (int x = 0, DataLength = Length; x < DataLength; ++x)
            {
                Data.Add(DummyData[x]);
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            DummyData = new List<TestItem>();
            for (int x = 0; x < Length; ++x)
            {
                DummyData.Add(new TestItem { Name = "a" });
            }
        }

        private class TestItem
        {
            public string Name { get; set; }
        }
    }
}