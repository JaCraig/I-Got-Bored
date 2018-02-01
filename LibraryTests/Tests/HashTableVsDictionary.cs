using BenchmarkDotNet.Attributes;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace LibraryTests.Tests
{
    public class HashTableVsDictionary
    {
        [Benchmark(Description = "Dictionary<int,string>")]
        public void DictionaryIntTest()
        {
            var TestItem = new Dictionary<int, string>
            {
                { 0, "Value" }
            };
            var Value = TestItem[0];
        }

        [Benchmark(Baseline = true, Description = "Dictionary<string,string>")]
        public void DictionaryTest()
        {
            var TestItem = new Dictionary<string, string>
            {
                { "Key", "Value" }
            };
            var Value = TestItem["Key"];
        }

        [Benchmark(Description = "Hashtable (int,string)")]
        public void HashtableIntTest()
        {
            var TestItem = new Hashtable
            {
                { 0, "Value" }
            };
            string Value = (string)TestItem[0];
        }

        [Benchmark(Description = "Hashtable (string,string)")]
        public void HashtableTest()
        {
            var TestItem = new Hashtable
            {
                { "Key", "Value" }
            };
            string Value = (string)TestItem["Key"];
        }

        [Benchmark(Description = "StringDictionary")]
        public void StringDictionaryTest()
        {
            var TestItem = new StringDictionary
            {
                { "Key", "Value" }
            };
            var Value = TestItem["Key"];
        }
    }
}