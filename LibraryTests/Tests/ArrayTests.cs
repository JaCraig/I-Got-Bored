using BenchmarkDotNet.Attributes;
using System;

namespace LibraryTests.Tests
{
    public class ArrayCopyTests
    {
        [Params(100, 1000, 10000)]
        public int Count { get; set; }

        private byte[] source, destination;

        [Benchmark(Baseline = true, Description = "Copy using Array.Copy()")]
        public void CopyArray()
        {
            Array.Copy(source, destination, Count);
        }

        [Benchmark(Description = "Copy using Buffer.MemoryCopy<T>")]
        public unsafe void CopyUsingBufferMemoryCopy()
        {
            fixed (byte* pinnedDestination = destination)
            fixed (byte* pinnedSource = source)
            {
                Buffer.MemoryCopy(pinnedSource, pinnedDestination, Count, Count);
            }
        }

        [GlobalSetup]
        public void SetUp()
        {
            source = new byte[Count];
            destination = new byte[Count];
        }
    }
}