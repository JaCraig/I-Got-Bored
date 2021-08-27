using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System.Collections.Generic;
using System.Linq;

namespace LibraryTests.Tests
{
    public class IEnumerableConversion : TestBaseClass
    {
        private int Taking { get; set; }
        private IEnumerable<int> Data;

        [Benchmark(Description = "Array")]
        public void Array()
        {
            var ListData = Data.ToArray();
            var Result = Data.Take(Taking).Skip(10).ToList();
        }

        [Benchmark(Description = "Array Copy")]
        public void ArrayCopyTo()
        {
            var ListData = Data.ToArray();
            var ResultArray = new int[Taking];
            System.Array.Copy(ListData, 10, ResultArray, 0, Taking);
            var Result = ResultArray.ToList();
        }

        [Benchmark(Description = "Array for loop")]
        public void ArrayForLoop()
        {
            var ListData = Data.ToArray();
            var Result = new List<int>();
            for (int x = 10; x < Taking; ++x)
            {
                Result.Add(ListData[x]);
            }
        }

        [Benchmark(Baseline = true, Description = "IEnumerable")]
        public void IEnumerable()
        {
            var Result = Data.Take(Taking).Skip(10).ToList();
        }

        [Benchmark(Description = "List")]
        public void List()
        {
            var ListData = Data.ToList();
            var Result = Data.Take(Taking).Skip(10).ToList();
        }

        [Benchmark(Description = "List CopyTo")]
        public void ListCopyTo()
        {
            var ListData = Data.ToList();
            var ResultArray = new int[Taking];
            ListData.CopyTo(10, ResultArray, 0, Taking);
            var Result = ResultArray.ToList();
        }

        [Benchmark(Description = "List for loop")]
        public void ListForLoop()
        {
            var ListData = Data.ToList();
            var Result = new List<int>();
            for (int x = 10; x < Taking; ++x)
            {
                Result.Add(ListData[x]);
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            Taking = Count / 2;
            var TempData = new List<int>();
            for (int x = 0; x < Count; ++x)
            {
                TempData.Add(x);
            }
            Data = TempData.Where(x => x > Taking / 2);
        }
    }
}