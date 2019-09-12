using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Text;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, RankColumn]
    public class StringConcatTests
    {
        [Params(100, 1000, 10000, 100000)]
        public int Count { get; set; }

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