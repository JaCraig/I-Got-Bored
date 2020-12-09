using BenchmarkDotNet.Attributes;
using System;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class SpanVsArray
    {
        [Benchmark(Baseline = true, Description = "Array")]
        public unsafe void ArrayTest()
        {
            int[] Data = new int[10000];
            for (int y = 0; y < 100; ++y)
            {
                for (int x = 0; x < 100; ++x)
                {
                    Data[(y * 100) + x] = 1;
                }
            }
        }

        [Benchmark(Description = "Array With Pointers")]
        public unsafe void ArrayWithPointersTest()
        {
            int[] Data = new int[10000];
            fixed (int* OriginalPointer = &Data[0])
            {
                int* Pointer = OriginalPointer;
                for (int y = 0; y < 100; ++y)
                {
                    Pointer = OriginalPointer + (y * 100);
                    for (int x = 0; x < 100; ++x)
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
            int[] Data = new int[10000];
            Span<int> Temp = Data;
            for (int y = 0; y < 100; ++y)
            {
                var YPos = y * 100;
                for (int x = 0; x < 100; ++x)
                {
                    Temp[YPos + x] = 1;
                }
            }
        }

        [Benchmark(Description = "Span With Slice")]
        public unsafe void SpanWithSliceTest()
        {
            int[] Data = new int[10000];
            Span<int> Temp = Data;
            for (int y = 0; y < 100; ++y)
            {
                var Slice = Temp.Slice(y * 100, 100);
                for (int x = 0; x < 100; ++x)
                {
                    Slice[x] = 1;
                }
            }
        }
    }
}