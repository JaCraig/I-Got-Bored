using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LibraryTests.Tests
{
    public class IEnumerableTests : TestBaseClass
    {
        public int[] ArrayData { get; set; }

        public ArrayList ArrayListData { get; set; }

        public ConcurrentBag<int> BagData { get; set; }

        public HashSet<int> HashSetData { get; set; }

        public List<int> ListData { get; set; }

        [Benchmark(Description = "ArrayList test")]
        public void ArrayListTest()
        {
            foreach (var Item in ArrayListData) { }
        }

        [Benchmark(Description = "Array test")]
        public void ArrayTest()
        {
            foreach (var Item in ArrayData) { }
        }

        [Benchmark(Description = "ConcurrentBag test")]
        public void ConcurrentBagTest()
        {
            foreach (var Item in BagData) { }
        }

        [Benchmark(Description = "HashSet test")]
        public void HashSetTest()
        {
            foreach (var Item in HashSetData) { }
        }

        [Benchmark(Baseline = true, Description = "List test")]
        public void ListTest()
        {
            foreach (var Item in ListData) { }
        }

        [GlobalSetup]
        public void Setup()
        {
            ListData = new List<int>();
            ArrayData = new int[Count];
            BagData = new ConcurrentBag<int>();
            HashSetData = new HashSet<int>();
            ArrayListData = new ArrayList();
            for (int x = 0; x < Count; ++x)
            {
                ArrayListData.Add(x);
                BagData.Add(x);
                ListData.Add(x);
                HashSetData.Add(x);
            }
        }
    }
}