using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LibraryTests.Tests
{
    public class LoopTests : TestBaseClass
    {
        public int[] TestData;

        [Benchmark(Description = "Foreach loop", Baseline = true)]
        public void ForEachLoop()
        {
            foreach (var Item in TestData)
            {
                HolderMethod(Item);
            }
        }

        [Benchmark(Description = "For loop")]
        public void ForLoop()
        {
            for (int x = 0; x < TestData.Length; ++x)
            {
                HolderMethod(TestData[x]);
            }
        }

        [Benchmark(Description = "For loop, local length")]
        public void ForLoopLocalLength()
        {
            int Length = TestData.Length;
            for (int x = 0; x < Length; ++x)
            {
                HolderMethod(TestData[x]);
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            TestData = new int[Count];
        }

        [Benchmark(Description = "For loop, using span")]
        public void SpanFor()
        {
            Span<int> ArraySpan = TestData;
            for (int x = 0; x < ArraySpan.Length; ++x)
            {
                HolderMethod(ArraySpan[x]);
            }
        }

        [Benchmark(Description = "For loop, using span and unsafe")]
        public void UnsafeSpanFor()
        {
            Span<int> ArraySpan = TestData;
            ref var SpanRef = ref MemoryMarshal.GetReference(ArraySpan);
            for (int x = 0; x < ArraySpan.Length; ++x)
            {
                HolderMethod(Unsafe.Add(ref SpanRef, x));
            }
        }

        private void HolderMethod(int x)
        { }
    }
}