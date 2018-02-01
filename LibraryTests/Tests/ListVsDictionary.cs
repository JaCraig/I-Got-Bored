using BenchmarkDotNet.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace LibraryTests.Tests
{
    public class ListVsDictionary
    {
        private ArrayList ArrayListData;
        private Collection<TestClass> CollectionData;
        private Dictionary<string, TestClass> DictionaryData;

        private HashSet<TestClass> HashData;
        private ImmutableDictionary<string, TestClass> ImmutableDictionaryData;
        private ImmutableList<TestClass> ImmutableListData;
        private List<TestClass> ListData;

        [Benchmark(Description = "ArrayList for loop")]
        public void ArrayListForLoop()
        {
            TestClass Value = null;
            for (int x = 0; x < ArrayListData.Count; ++x)
            {
                TestClass TempValue = (TestClass)ArrayListData[x];
                if (TempValue.Key == "100")
                {
                    Value = TempValue;
                    break;
                }
            }
        }

        [Benchmark(Description = "Collection.First")]
        public void CollectionFirst()
        {
            var Value = CollectionData.First(x => x.Key == "100");
        }

        [Benchmark(Description = "Collection.FirstOrDefault")]
        public void CollectionFirstOrDefault()
        {
            var Value = CollectionData.FirstOrDefault(x => x.Key == "100");
        }

        [Benchmark(Description = "Collection for loop")]
        public void CollectionForLoop()
        {
            TestClass Value = null;
            for (int x = 0; x < CollectionData.Count; ++x)
            {
                if (CollectionData[x].Key == "100")
                {
                    Value = CollectionData[x];
                    break;
                }
            }
        }

        [Benchmark(Description = "Dictionary[]")]
        public void Dictionary()
        {
            var Value = DictionaryData["100"];
        }

        [Benchmark(Description = "Dictionary.TryGetValue")]
        public void DictionaryTryGetValue()
        {
            DictionaryData.TryGetValue("100", out TestClass Value);
        }

        [Benchmark(Description = "HashSet.First")]
        public void HashSetFirst()
        {
            var Value = HashData.First(x => x.Key == "100");
        }

        [Benchmark(Description = "HashSet.FirstOrDefault")]
        public void HashSetFirstOrDefault()
        {
            var Value = HashData.FirstOrDefault(x => x.Key == "100");
        }

        [Benchmark(Description = "ImmutableDictionary[]")]
        public void ImmutableDictionary()
        {
            var Value = ImmutableDictionaryData["100"];
        }

        [Benchmark(Description = "ImmutableDictionary.TryGetValue")]
        public void ImmutableDictionaryTryGetValue()
        {
            ImmutableDictionaryData.TryGetValue("100", out TestClass Value);
        }

        [Benchmark(Description = "ImmutableList.First")]
        public void ImmutableListFirst()
        {
            var Value = ImmutableListData.First(x => x.Key == "100");
        }

        [Benchmark(Description = "ImmutableList.FirstOrDefault")]
        public void ImmutableListFirstOrDefault()
        {
            var Value = ImmutableListData.FirstOrDefault(x => x.Key == "100");
        }

        [Benchmark(Description = "ImmutableList for loop")]
        public void ImmutableListForLoop()
        {
            TestClass Value = null;
            for (int x = 0; x < ImmutableListData.Count; ++x)
            {
                if (ImmutableListData[x].Key == "100")
                {
                    Value = ImmutableListData[x];
                    break;
                }
            }
        }

        [Benchmark(Baseline = true, Description = "List.First")]
        public void ListFirst()
        {
            var Value = ListData.First(x => x.Key == "100");
        }

        [Benchmark(Description = "List.FirstOrDefault")]
        public void ListFirstOrDefault()
        {
            var Value = ListData.FirstOrDefault(x => x.Key == "100");
        }

        [Benchmark(Description = "List for loop")]
        public void ListForLoop()
        {
            TestClass Value = null;
            for (int x = 0; x < ListData.Count; ++x)
            {
                if (ListData[x].Key == "100")
                {
                    Value = ListData[x];
                    break;
                }
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            HashData = new HashSet<TestClass>();
            ListData = new List<TestClass>();
            DictionaryData = new Dictionary<string, TestClass>();
            ArrayListData = new ArrayList();
            CollectionData = new Collection<TestClass>();
            for (int x = 0; x < 10000; ++x)
            {
                CollectionData.Add(new TestClass { Value = x, Key = x.ToString() });
                ArrayListData.Add(new TestClass { Value = x, Key = x.ToString() });
                HashData.Add(new TestClass { Value = x, Key = x.ToString() });
                ListData.Add(new TestClass { Value = x, Key = x.ToString() });
                DictionaryData.Add(x.ToString(), new TestClass { Value = x, Key = x.ToString() });
            }
            ImmutableDictionaryData = DictionaryData.ToImmutableDictionary();
            ImmutableListData = ListData.ToImmutableList();
        }

        private class TestClass
        {
            public string Key { get; set; }
            public int Value { get; set; }

            public override bool Equals(object obj)
            {
                var @class = obj as TestClass;
                return @class != null &&
                       Key == @class.Key;
            }

            public override int GetHashCode()
            {
                return 990326508 + EqualityComparer<string>.Default.GetHashCode(Key);
            }
        }
    }
}