using BenchmarkDotNet.Attributes;
using JM.LinqFaster;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class ArraySortTests
    {
        private int[] Data;

        private List<int> ListData;

        [Params(10, 100, 1000, 10000, 100000)]
        public int Size { get; set; }

        [Benchmark(Description = "Linq Array.OrderBy()", Baseline = true)]
        public void ArrayOrderBy()
        {
            var SortedData = Data.OrderBy(x => x).ToArray();
        }

        [Benchmark(Description = "Array.OrderByF()")]
        public void ArrayOrderByF()
        {
            var SortedData = Data.OrderByF(x => x);
        }

        [Benchmark(Description = "PLinq Array.AsParallel().OrderBy()")]
        public void ArrayParallelOrderBy()
        {
            var SortedData = Data.AsParallel().OrderBy(x => x).ToArray();
        }

        [Benchmark(Description = "Array.Sort()")]
        public void ArraySort()
        {
            Array.Sort(Data);
            var SortedData = Data;
        }

        [Benchmark(Description = "List.OrderBy")]
        public void ListOrderBy()
        {
            var Results = ListData.OrderBy(x => x).ToList();
        }

        [Benchmark(Description = "List.OrderByF")]
        public void ListOrderByFaster()
        {
            var Results = ListData.OrderByF(x => x);
        }

        [Benchmark(Description = "List.Sort()")]
        public void ListSort()
        {
            ListData.Sort();
            var SortedData = ListData;
        }

        [GlobalSetup]
        public void SetUp()
        {
            Data = new int[Size];
            ListData = new List<int>();
            var RandomObj = new Random();
            for (int x = 0; x < Data.Length; ++x)
            {
                Data[x] = RandomObj.Next();
                ListData.Add(Data[x]);
            }
        }
    }
}