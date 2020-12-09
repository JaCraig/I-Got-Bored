using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class ClassVsStructCreation
    {
        [Benchmark(Baseline = true, Description = "Class test")]
        public void ClassTest()
        {
            var Value = new TestClass[10000];
        }

        [Benchmark(Description = "Struct test")]
        public void StructTest()
        {
            var Value = new TestStruct[10000];
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