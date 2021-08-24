using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LibraryTests.Tests
{
    public class PartitionedParallel : TestBaseClass
    {
        private byte[] Data;

        [Benchmark(Baseline = true, Description = "Parallel.For")]
        public void ParallelFor()
        {
            int sum = 0;
            var RangePartitioner = Partitioner.Create(0, Data.Length);
            Parallel.For(0, Data.Length,
                () => 0,
                (value, state, acc) =>
               {
                   acc += value;
                   return acc;
               },
            acc => Interlocked.Add(ref sum, acc));
        }

        [Benchmark(Description = "Parallel.ForEach")]
        public void ParallelForEach()
        {
            int sum = 0;
            Parallel.ForEach(Data,
                () => 0,
                (value, state, acc) =>
                {
                    acc += value;
                    return acc;
                },
            acc => Interlocked.Add(ref sum, acc));
        }

        [Benchmark(Description = "Parallel.ForEach partitioned")]
        public void ParallelForEachPartitioned()
        {
            int sum = 0;
            var RangePartitioner = Partitioner.Create(0, Data.Length);
            Parallel.ForEach(RangePartitioner,
                () => 0,
                (range, s, acc) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        acc += Data[i];
                    }
                    return acc;
                },
            acc => Interlocked.Add(ref sum, acc));
        }

        [GlobalSetup]
        public void Setup()
        {
            Data = new byte[Count];
        }
    }
}