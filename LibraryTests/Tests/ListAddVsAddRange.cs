using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System.Collections.Generic;

namespace LibraryTests.Tests
{
    public class ListAddVsAddRange : TestBaseClass
    {
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
            for (int x = 0, DataLength = Count; x < DataLength; ++x)
            {
                Data.Add(DummyData[x]);
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            DummyData = new List<TestItem>();
            for (int x = 0; x < Count; ++x)
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