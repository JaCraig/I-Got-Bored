using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class StackAllocVsNew : TestBaseClass
    {
        [Benchmark(Description = "New")]
        public unsafe void NewTest()
        {
            int[] Data = new int[Count];
            fixed (int* Fib = &Data[0])
            {
                int* Pointer = Fib;
                *Pointer++ = *Pointer++ = 1;
                for (int x = 2; x < Count; ++x, ++Pointer)
                {
                    *Pointer = Pointer[-1] + Pointer[-2];
                }
            }
        }

        [Benchmark(Description = "Stackalloc struct")]
        public unsafe void StackAllocClassTest()
        {
            TempClass* Fib = stackalloc TempClass[Count];
            TempClass* Pointer = Fib;
            (*Pointer++).Data = 1;
            (*Pointer++).Data = 1;
            for (int x = 2; x < Count; ++x, ++Pointer)
            {
                (*Pointer).Data = Pointer[-1].Data + Pointer[-2].Data;
            }
        }

        [Benchmark(Baseline = true, Description = "Stackalloc")]
        public unsafe void StackAllocTest()
        {
            int* Fib = stackalloc int[Count];
            int* Pointer = Fib;
            *Pointer++ = *Pointer++ = 1;
            for (int x = 2; x < Count; ++x, ++Pointer)
            {
                *Pointer = Pointer[-1] + Pointer[-2];
            }
        }

        public struct TempClass
        {
            public int Data;
        }
    }
}