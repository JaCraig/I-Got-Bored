using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class PatternMatchingTests
    {
        [Benchmark(Description = "As")]
        public void AsTest()
        {
            var Item = (object)new TestClass();
            var Result = Item as TestClass;
            if (Result != null)
            {
                Result.Property1 = 1;
            }
        }

        [Benchmark(Description = "Is")]
        public void IsTest()
        {
            var Item = (object)new TestClass();
            if (Item is TestClass)
            {
                var Result = (TestClass)Item;
                Result.Property1 = 1;
            }
        }

        [Benchmark(Description = "Pattern matching", Baseline = true)]
        public void PatternMatchingTest()
        {
            var Item = (object)new TestClass();
            if (Item is TestClass Result)
            {
                Result.Property1 = 1;
            }
        }

        private class TestClass
        {
            public int Property1 { get; set; }
        }
    }
}