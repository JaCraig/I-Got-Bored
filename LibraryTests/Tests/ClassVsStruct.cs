using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class ClassVsStructCreation : TestBaseClass
    {
        [Benchmark(Baseline = true, Description = "Class test")]
        public void ClassTest()
        {
            var Value = new TestClass[Count];
            for (var x = 0; x < Count; ++x)
            {
                Value[x] = new TestClass();
            }
        }

        [Benchmark(Description = "Struct test")]
        public void StructTest()
        {
            var Value = new TestStruct[Count];
            for (var x = 0; x < Count; ++x)
            {
                Value[x] = new TestStruct();
            }
        }

        private struct TestStruct
        {
            public int Data { get; set; }
        }

        private class TestClass
        {
            public int Data { get; set; }
        }
    }
}