using BenchmarkDotNet.Attributes;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class DictionaryVsConcurrentDictionary
    {
        [Benchmark(Description = "ConcurrentDictionary<string,string>")]
        public void ConcurrentDictionaryTest()
        {
            ConcurrentDictionary<string, string> TestObject = new ConcurrentDictionary<string, string>();
            TestObject.AddOrUpdate("Key", "Value", (x, y) => x);
            TestObject.ContainsKey("Key");
            var Value = TestObject["Key"];
            TestObject["Key"] = Value;
        }

        [Benchmark(Baseline = true, Description = "Dictionary<string,string>")]
        public void DictionaryTest()
        {
            Dictionary<string, string> TestObject = new Dictionary<string, string>
            {
                { "Key", "Value" }
            };
            TestObject.ContainsKey("Key");
            var Value = TestObject["Key"];
            TestObject["Key"] = Value;
        }
    }
}