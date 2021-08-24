using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System;
using System.Buffers;

namespace LibraryTests.Tests
{
    public class AllocationStrategyTests : TestBaseClass
    {
        private ColorStruct[] DataPool;

        [Benchmark(Baseline = true, Description = "Allocate as needed")]
        public void AllocateAsNeeded()
        {
            ColorStruct[] Items = new ColorStruct[Count];
            for (int x = 0; x < Count; ++x)
            {
                ColorStruct Item = Items[x];
                Item.Alpha = 1;
                Item.Blue = 1;
                Item.Green = 1;
                Item.Red = 1;
            }
        }

        [Benchmark(Description = "ArrayPool")]
        public void ArrayPoolTest()
        {
            var Pool = ArrayPool<ColorStruct>.Shared;
            var Items = Pool.Rent(Count);
            for (int x = 0; x < Count; ++x)
            {
                ColorStruct Item = Items[x];
                Item.Alpha = 1;
                Item.Blue = 1;
                Item.Green = 1;
                Item.Red = 1;
            }
            Pool.Return(Items);
        }

        [Benchmark(Description = "Item from pointer to preallocated array 'pool'")]
        public unsafe void PointerToPool()
        {
            fixed (ColorStruct* DataPointer = DataPool)
            {
                ColorStruct* ItemPointer = DataPointer;
                for (int x = 0; x < DataPool.Length; ++x)
                {
                    (*ItemPointer).Alpha = 1;
                    (*ItemPointer).Blue = 1;
                    (*ItemPointer).Green = 1;
                    (*ItemPointer).Red = 1;
                    ItemPointer++;
                }
            }
        }

        [GlobalSetup]
        public void SetUp()
        {
            DataPool = new ColorStruct[Count];
        }

        [Benchmark(Description = "Span from preallocated array 'pool'")]
        public void SpanFromPool()
        {
            var SpanPool = new Span<ColorStruct>(DataPool);
            for (int x = 0; x < SpanPool.Length; ++x)
            {
                SpanPool[x].Alpha = 1;
                SpanPool[x].Blue = 1;
                SpanPool[x].Green = 1;
                SpanPool[x].Red = 1;
            }
        }
    }
}