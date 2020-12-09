using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class IncrementTests
    {
        [Benchmark(Description = "x+=1")]
        public void PlusEqualsIncrement()
        {
            for (int x = 0; x < 100000; x += 1) { }
        }

        [Benchmark(Description = "x++", Baseline = true)]
        public void PostIncrement()
        {
            for (int x = 0; x < 100000; x++) { }
        }

        [Benchmark(Description = "++x")]
        public void PreIncrement()
        {
            for (int x = 0; x < 100000; ++x) { }
        }
    }
}