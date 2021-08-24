using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class BitwiseVsModulo : TestBaseClass
    {
        [Benchmark(Description = "Bitwise Modulo")]
        public void BitwiseMultiplication()
        {
            var Value = 274162;
            for (int x = 0; x < Count; ++x)
            {
                var Result = Value & 10;
            }
        }

        [Benchmark(Baseline = true, Description = "Normal Modulo")]
        public void NormalMultiplication()
        {
            var Value = 274162;
            for (int x = 0; x < Count; ++x)
            {
                var Result = Value % 10;
            }
        }
    }
}