using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class LoopTests : TestBaseClass
    {
        public int[] TestData;

        [Benchmark(Description = "Foreach loop", Baseline = true)]
        public void ForEachLoop()
        {
            foreach (var Item in TestData) { }
        }

        [Benchmark(Description = "For loop")]
        public void ForLoop()
        {
            for (int x = 0; x < TestData.Length; ++x) { }
        }

        [Benchmark(Description = "For loop, local length")]
        public void ForLoopLocalLength()
        {
            int Length = TestData.Length;
            for (int x = 0; x < Length; ++x) { }
        }

        [GlobalSetup]
        public void Setup()
        {
            TestData = new int[Count];
        }
    }
}