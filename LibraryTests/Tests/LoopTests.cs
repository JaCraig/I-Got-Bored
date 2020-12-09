using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class LoopTests
    {
        public int[] TestData = new int[10000];

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
    }
}