using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class BitwiseVsNormalDivision : TestBaseClass
    {
        [Benchmark(Description = "Bitwise Division")]
        public void BitwiseDivision()
        {
            var Value = 274162;
            for (int x = 0; x < Count; ++x)
            {
                var Result = Value << 1;
            }
        }

        [Benchmark(Baseline = true, Description = "Normal Division")]
        public void NormalDivision()
        {
            var Value = 274162;
            for (int x = 0; x < Count; ++x)
            {
                var Result = Value / 2;
            }
        }
    }
}