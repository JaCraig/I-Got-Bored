using BenchmarkDotNet.Attributes;
using System.Collections.Generic;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class DictionaryKeyTests
    {
        [Benchmark(Description = "Dictionary<int,string>")]
        public void IntKey()
        {
            Dictionary<int, string> TestObject = new Dictionary<int, string>
            {
                { 1, "Value" }
            };
            TestObject.ContainsKey(1);
            var Value = TestObject[1];
            TestObject[1] = Value;
        }

        [Benchmark(Description = "Dictionary<long,string>")]
        public void LongKey()
        {
            Dictionary<long, string> TestObject = new Dictionary<long, string>
            {
                { 1, "Value" }
            };
            TestObject.ContainsKey(1);
            var Value = TestObject[1];
            TestObject[1] = Value;
        }

        [Benchmark(Description = "Dictionary<short,string>")]
        public void ShortKey()
        {
            Dictionary<short, string> TestObject = new Dictionary<short, string>
            {
                { 1, "Value" }
            };
            TestObject.ContainsKey(1);
            var Value = TestObject[1];
            TestObject[1] = Value;
        }

        [Benchmark(Description = "Dictionary<int,string> Using string key HashCode")]
        public void StringHashCodeKey()
        {
            Dictionary<int, string> TestObject = new Dictionary<int, string>
            {
                { "Key".GetHashCode(), "Value" }
            };
            TestObject.ContainsKey("Key".GetHashCode());
            var Value = TestObject["Key".GetHashCode()];
            TestObject["Key".GetHashCode()] = Value;
        }

        [Benchmark(Description = "Dictionary<string,string> key interned")]
        public void StringInternedKey()
        {
            Dictionary<string, string> TestObject = new Dictionary<string, string>
            {
                { string.Intern("Key"), "Value" }
            };
            TestObject.ContainsKey(string.Intern("Key"));
            var Value = TestObject[string.Intern("Key")];
            TestObject[string.Intern("Key")] = Value;
        }

        [Benchmark(Description = "Dictionary<string,string>", Baseline = true)]
        public void StringKey()
        {
            Dictionary<string, string> TestObject = new Dictionary<string, string>
            {
                { "Key", "Value" }
            };
            TestObject.ContainsKey("Key");
            var Value = TestObject["Key"];
            TestObject["Key"] = Value;
        }

        [Benchmark(Description = "Dictionary<uint,string>")]
        public void UIntKey()
        {
            Dictionary<uint, string> TestObject = new Dictionary<uint, string>
            {
                { 1, "Value" }
            };
            TestObject.ContainsKey(1);
            var Value = TestObject[1];
            TestObject[1] = Value;
        }

        [Benchmark(Description = "Dictionary<ulong,string>")]
        public void ULongKey()
        {
            Dictionary<ulong, string> TestObject = new Dictionary<ulong, string>
            {
                { 1, "Value" }
            };
            TestObject.ContainsKey(1);
            var Value = TestObject[1];
            TestObject[1] = Value;
        }

        [Benchmark(Description = "Dictionary<ushort,string>")]
        public void UShortKey()
        {
            Dictionary<ushort, string> TestObject = new Dictionary<ushort, string>
            {
                { 1, "Value" }
            };
            TestObject.ContainsKey(1);
            var Value = TestObject[1];
            TestObject[1] = Value;
        }
    }
}