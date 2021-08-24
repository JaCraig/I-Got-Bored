using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class ClassVsStructSet : TestBaseClass
    {
        private TestClass TempClass = new TestClass();
        private TestStruct TempStruct = new TestStruct();

        [Benchmark(Baseline = true, Description = "Class test")]
        public void ClassTest()
        {
            for (int x = 0; x < Count; ++x)
            {
                TempClass.Data = x;
            }
        }

        [Benchmark(Description = "Struct test")]
        public void StructTest()
        {
            for (int x = 0; x < Count; ++x)
            {
                TempStruct.Data = x;
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