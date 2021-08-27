using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System;

namespace LibraryTests.Tests
{
    public class SpanVsArray : TestBaseClass
    {
        private int LineLength;

        [Benchmark(Baseline = true, Description = "Array")]
        public unsafe void ArrayTest()
        {
            int[] Data = new int[Count];
            for (int y = 0; y < LineLength; ++y)
            {
                for (int x = 0; x < LineLength; ++x)
                {
                    Data[(y * (LineLength)) + x] = 1;
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
                for (int y = 0; y < LineLength; ++y)
                {
                    Pointer = OriginalPointer + (y * (LineLength));
                    for (int x = 0; x < LineLength; ++x)
                    {
                        *Pointer = 1;
                        ++Pointer;
                    }
                }
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            LineLength = (int)Math.Sqrt((double)Count);
        }

        [Benchmark(Description = "Span")]
        public unsafe void SpanTest()
        {
            Span<int> Temp = new int[Count];
            for (int y = 0; y < LineLength; ++y)
            {
                var YPos = y * LineLength;
                for (int x = 0; x < LineLength; ++x)
                {
                    Temp[YPos + x] = 1;
                }
            }
        }

        [Benchmark(Description = "Span With Slice")]
        public unsafe void SpanWithSliceTest()
        {
            Span<int> Temp = new int[Count];
            for (int y = 0; y < LineLength; ++y)
            {
                var Slice = Temp.Slice(y * (LineLength), LineLength);
                for (int x = 0; x < LineLength; ++x)
                {
                    Slice[x] = 1;
                }
            }
        }
    }
}