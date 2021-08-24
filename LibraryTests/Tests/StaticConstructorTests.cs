using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System;

namespace LibraryTests.Tests
{
    public class StaticConstructorTests : TestBaseClass
    {
        [Benchmark(Baseline = true, Description = "No constructor")]
        public void WithoutStaticConstructor()
        {
            for (int x = 0; x < Count; ++x)
            {
                var Value = NoConstructor.Number;
            }
        }

        [Benchmark(Description = "With constructor")]
        public void WithStaticConstructor()
        {
            for (int x = 0; x < Count; ++x)
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