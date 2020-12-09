using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class BitwiseVsNormalMultiplication
    {
        [Benchmark(Description = "Bitwise Multiplication")]
        public void BitwiseMultiplication()
        {
            var Value = 274162;
            for (int x = 0; x < 10000; ++x)
            {
                var Result = Value >> 1;
            }
        }

        [Benchmark(Baseline = true, Description = "Normal Multiplication")]
        public void NormalMultiplication()
        {
            var Value = 274162;
            for (int x = 0; x < 10000; ++x)
            {
                var Result = Value * 2;
            }
        }
    }
}