using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System;

namespace LibraryTests.Tests
{
    public class SpanVsArray : TestBaseClass
    {
        [Benchmark(Baseline = true, Description = "Array")]
        public unsafe void ArrayTest()
        {
            int[] Data = new int[Count];
            for (int y = 0; y < Count / 10; ++y)
            {
                for (int x = 0; x < Count / 10; ++x)
                {
                    Data[(y * (Count / 10)) + x] = 1;
                }
            }
        }

        [Benchmark(Description = "Array With Pointers")]
        public unsafe void ArrayWithPointersTest()
        {
            int[] Data = new int[Count];
            fixed (int* OriginalPointer = &Data[0])
            {
                int* Pointer = OriginalPointer;
                for (int y = 0; y < Count / 10; ++y)
                {
                    Pointer = OriginalPointer + (y * (Count / 10));
                    for (int x = 0; x < Count / 10; ++x)
                    {
                        *Pointer = 1;
                        ++Pointer;
                    }
                }
            }
        }

        [Benchmark(Description = "Span")]
        public unsafe void SpanTest()
        {
            Span<int> Temp = new int[Count];
            for (int y = 0; y < Count / 10; ++y)
            {
                var YPos = y * Count / 10;
                for (int x = 0; x < Count; ++x)
                {
                    Temp[YPos + x] = 1;
                }
            }
        }

        [Benchmark(Description = "Span With Slice")]
        public unsafe void SpanWithSliceTest()
        {
            Span<int> Temp = new int[Count];
            for (int y = 0; y < Count / 10; ++y)
            {
                var Slice = Temp.Slice(y * (Count / 10), Count / 10);
                for (int x = 0; x < Count / 10; ++x)
                {
                    Slice[x] = 1;
                }
            }
        }
    }
}