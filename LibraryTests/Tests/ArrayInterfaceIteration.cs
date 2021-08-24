using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System.Collections;
using System.Collections.Generic;

namespace LibraryTests.Tests
{
    public class ArrayInterfaceIteration : TestBaseClass
    {
        public int[] Data;

        [Benchmark(Description = "T[] ForEach")]
        public void ArrayForEachTest()
        {
            int[] LocalData = Data;
            foreach (var Item in LocalData)
            {
                var TempValue = Item;
            }
        }

        [Benchmark(Description = "T[] For", Baseline = true)]
        public void ArrayTest()
        {
            int[] LocalData = Data;
            for (int x = 0, DataLength = LocalData.Length; x < DataLength; ++x)
            {
                var TempValue = LocalData[x];
            }
        }

        [Benchmark(Description = "ICollection ForEach")]
        public void ICollectionForEachTest()
        {
            ICollection LocalData = Data;
            foreach (var Item in LocalData)
            {
                var TempValue = Item;
            }
        }

        [Benchmark(Description = "ICollection<T> ForEach")]
        public void ICollectionGenericForEachTest()
        {
            ICollection<int> LocalData = Data;
            foreach (var Item in LocalData)
            {
                var TempValue = Item;
            }
        }

        [Benchmark(Description = "IEnumerable ForEach")]
        public void IEnumerableForEachTest()
        {
            IEnumerable LocalData = Data;
            foreach (var Item in LocalData)
            {
                var TempValue = Item;
            }
        }

        [Benchmark(Description = "IEnumerable<T> ForEach")]
        public void IEnumerableGenericForEachTest()
        {
            IEnumerable<int> LocalData = Data;
            foreach (var Item in LocalData)
            {
                var TempValue = Item;
            }
        }

        [Benchmark(Description = "IList ForEach")]
        public void IListForEachTest()
        {
            IList LocalData = Data;
            foreach (var Item in LocalData)
            {
                var TempValue = Item;
            }
        }

        [Benchmark(Description = "IList<T> ForEach")]
        public void IListGenericForEachTest()
        {
            IList<int> LocalData = Data;
            foreach (var Item in LocalData)
            {
                var TempValue = Item;
            }
        }

        [Benchmark(Description = "IList<T> For")]
        public void IListGenericTest()
        {
            IList<int> LocalData = Data;
            for (int x = 0, DataLength = LocalData.Count; x < DataLength; ++x)
            {
                var TempValue = LocalData[x];
            }
        }

        [Benchmark(Description = "IList For")]
        public void IListTest()
        {
            IList LocalData = Data;
            for (int x = 0, DataLength = LocalData.Count; x < DataLength; ++x)
            {
                var TempValue = LocalData[x];
            }
        }

        [Benchmark(Description = "IReadOnlyCollection<T> ForEach")]
        public void IReadOnlyCollectionForEachTest()
        {
            IReadOnlyCollection<int> LocalData = Data;
            foreach (var Item in LocalData)
            {
                var TempValue = Item;
            }
        }

        [Benchmark(Description = "IReadOnlyList<T> ForEach")]
        public void IReadOnlyListForEachTest()
        {
            IReadOnlyList<int> LocalData = Data;
            foreach (var Item in LocalData)
            {
                var TempValue = Item;
            }
        }

        [Benchmark(Description = "IReadOnlyList<T> For")]
        public void IReadOnlyListTest()
        {
            IReadOnlyList<int> LocalData = Data;
            for (int x = 0, DataLength = LocalData.Count; x < DataLength; ++x)
            {
                var TempValue = LocalData[x];
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            Data = new int[Count];
        }
    }
}