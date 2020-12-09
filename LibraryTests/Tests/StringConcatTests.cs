using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.ObjectPool;
using System.Collections.Generic;
using System.Text;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, RankColumn, HtmlExporter]
    public class StringConcatTests
    {
        [Params(100, 1000, 10000, 100000)]
        public int Count { get; set; }

        private ObjectPool<StringBuilder> InternalStringBuilderPool { get; set; }

        [Benchmark(Description = "char list concat")]
        public string CharConcat()
        {
            List<char> Builder = new List<char>();
            for (int x = 0; x < Count; ++x)
            {
                Builder.Add('A');
                Builder.Add('S');
                Builder.Add('D');
                Builder.Add('F');
            }
            return new string(Builder.ToArray());
        }

        [GlobalSetup]
        public void Setup()
        {
            InternalStringBuilderPool = new DefaultObjectPoolProvider().CreateStringBuilderPool();
        }

        [Benchmark(Description = "StringBuilder")]
        public string StringBuilder()
        {
            StringBuilder Builder = new System.Text.StringBuilder();
            for (int x = 0; x < Count; ++x)
            {
                Builder.Append("ASDF");
            }
            return Builder.ToString();
        }

        [Benchmark(Description = "StringBuilderPool")]
        public string StringBuilderPool()
        {
            StringBuilder Builder = InternalStringBuilderPool.Get();
            for (int x = 0; x < Count; ++x)
            {
                Builder.Append("ASDF");
            }
            var Result = Builder.ToString();
            InternalStringBuilderPool.Return(Builder);
            return Result;
        }

        [Benchmark(Description = "string concat", Baseline = true)]
        public string StringConcat()
        {
            string Builder = "";
            for (int x = 0; x < Count; ++x)
            {
                Builder += "ASDF";
            }
            return Builder;
        }
    }
}