using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    public class CheckedVsUncheckedMath
    {
        [Benchmark(Baseline = true, Description = "Checked math")]
        public void Checked()
        {
            var Value = 274162;
            for (int x = 0; x < 10000; ++x)
            {
                var Result = Value * 2;
            }
        }

        [Benchmark(Description = "Unchecked math")]
        public void UnChecked()
        {
            var Value = 274162;
            unchecked
            {
                for (int x = 0; x < 10000; ++x)
                {
                    var Result = Value * 2;
                }
            }
        }
    }
}