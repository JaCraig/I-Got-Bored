using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class IEnumerableConversion
    {
        private IEnumerable<int> Data;

        [Benchmark(Description = "Array")]
        public void Array()
        {
            var ListData = Data.ToArray();
            var Result = Data.Take(500).Skip(10).ToList();
        }

        [Benchmark(Description = "Array Copy")]
        public void ArrayCopyTo()
        {
            var ListData = Data.ToArray();
            var ResultArray = new int[490];
            System.Array.Copy(ListData, 10, ResultArray, 0, 490);
            var Result = ResultArray.ToList();
        }

        [Benchmark(Description = "Array for loop")]
        public void ArrayForLoop()
        {
            var ListData = Data.ToArray();
            var Result = new List<int>();
            for (int x = 10; x < 500; ++x)
            {
                Result.Add(ListData[x]);
            }
        }

        [Benchmark(Baseline = true, Description = "IEnumerable")]
        public void IEnumerable()
        {
            var Result = Data.Take(500).Skip(10).ToList();
        }

        [Benchmark(Description = "List")]
        public void List()
        {
            var ListData = Data.ToList();
            var Result = Data.Take(500).Skip(10).ToList();
        }

        [Benchmark(Description = "List CopyTo")]
        public void ListCopyTo()
        {
            var ListData = Data.ToList();
            var ResultArray = new int[490];
            ListData.CopyTo(10, ResultArray, 0, 490);
            var Result = ResultArray.ToList();
        }

        [Benchmark(Description = "List for loop")]
        public void ListForLoop()
        {
            var ListData = Data.ToList();
            var Result = new List<int>();
            for (int x = 10; x < 500; ++x)
            {
                Result.Add(ListData[x]);
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            var TempData = new List<int>();
            for (int x = 0; x < 10000; ++x)
            {
                TempData.Add(x);
            }
            Data = TempData.Where(x => x > 5000);
        }
    }
}