using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class ClassVsStructSet
    {
        private TestClass TempClass = new TestClass();
        private TestStruct TempStruct = new TestStruct();

        [Benchmark(Baseline = true, Description = "Class test")]
        public void ClassTest()
        {
            for (int x = 0; x < 10000; ++x)
            {
                TempClass.Data = x;
            }
        }

        [Benchmark(Description = "Struct test")]
        public void StructTest()
        {
            for (int x = 0; x < 10000; ++x)
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