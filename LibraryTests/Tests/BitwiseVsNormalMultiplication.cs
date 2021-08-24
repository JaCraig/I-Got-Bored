using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class BitwiseVsNormalMultiplication : TestBaseClass
    {
        [Benchmark(Description = "Bitwise Multiplication")]
        public void BitwiseMultiplication()
        {
            var Value = 274162;
            for (int x = 0; x < Count; ++x)
            {
                var Result = Value >> 1;
            }
        }

        [Benchmark(Baseline = true, Description = "Normal Multiplication")]
        public void NormalMultiplication()
        {
            var Value = 274162;
            for (int x = 0; x < Count; ++x)
            {
                var Result = Value * 2;
            }
        }
    }
}