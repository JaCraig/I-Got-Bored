using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class IncrementTests : TestBaseClass
    {
        [Benchmark(Description = "x+=1")]
        public void PlusEqualsIncrement()
        {
            for (int x = 0; x < Count; x += 1) { }
        }

        [Benchmark(Description = "x++", Baseline = true)]
        public void PostIncrement()
        {
            for (int x = 0; x < Count; x++) { }
        }

        [Benchmark(Description = "++x")]
        public void PreIncrement()
        {
            for (int x = 0; x < Count; ++x) { }
        }
    }
}