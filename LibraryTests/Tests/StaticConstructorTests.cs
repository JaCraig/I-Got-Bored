using BenchmarkDotNet.Attributes;
using System;

namespace LibraryTests.Tests
{
    public class StaticConstructorTests
    {
        [Benchmark(Baseline = true, Description = "No constructor")]
        public void WithoutStaticConstructor()
        {
            for (int x = 0; x < 1000; ++x)
            {
                var Value = NoConstructor.Number;
            }
        }

        [Benchmark(Description = "With constructor")]
        public void WithStaticConstructor()
        {
            for (int x = 0; x < 1000; ++x)
            {
                var Value = Constructor.Number;
            }
        }

        private static class Constructor
        {
            static Constructor()
            {
                Number = Environment.ProcessorCount;
            }

            public static readonly int Number;
        }

        private static class NoConstructor
        {
            public static readonly int Number = Environment.ProcessorCount;
        }
    }
}