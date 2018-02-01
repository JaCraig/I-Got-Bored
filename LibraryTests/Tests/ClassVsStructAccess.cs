using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    public class ClassVsStructAccess
    {
        private TestClass TempClass = new TestClass();
        private TestStruct TempStruct = new TestStruct();

        [Benchmark(Baseline = true, Description = "Class test")]
        public void ClassTest()
        {
            for (int x = 0; x < 10000; ++x)
            {
                var Value = TempClass.Data;
            }
        }

        [Benchmark(Description = "Struct test")]
        public void StructTest()
        {
            for (int x = 0; x < 10000; ++x)
            {
                var Value = TempStruct.Data;
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