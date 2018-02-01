using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    public class StackAllocVsNew
    {
        [Benchmark(Description = "New")]
        public unsafe void NewTest()
        {
            int[] Data = new int[20000];
            fixed (int* Fib = &Data[0])
            {
                int* Pointer = Fib;
                *Pointer++ = *Pointer++ = 1;
                for (int x = 2; x < 20000; ++x, ++Pointer)
                {
                    *Pointer = Pointer[-1] + Pointer[-2];
                }
            }
        }

        [Benchmark(Description = "Stackalloc struct")]
        public unsafe void StackAllocClassTest()
        {
            TempClass* Fib = stackalloc TempClass[20000];
            TempClass* Pointer = Fib;
            (*Pointer++).Data = 1;
            (*Pointer++).Data = 1;
            for (int x = 2; x < 20000; ++x, ++Pointer)
            {
                (*Pointer).Data = Pointer[-1].Data + Pointer[-2].Data;
            }
        }

        [Benchmark(Baseline = true, Description = "Stackalloc")]
        public unsafe void StackAllocTest()
        {
            int* Fib = stackalloc int[20000];
            int* Pointer = Fib;
            *Pointer++ = *Pointer++ = 1;
            for (int x = 2; x < 20000; ++x, ++Pointer)
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