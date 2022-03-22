﻿using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System.Collections.Generic;
using System.Text;

namespace LibraryTests.Tests
{
    public class StringConcatTests : TestBaseClass
    {
        private string[] Values;

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
            Values = new string[Count];
            for (var x = 0; x < Count; ++x)
            {
                Values[x] = "ASDF";
            }
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

        [Benchmark(Description = "string.concat")]
        public string StringDotConcat()
        {
            return string.Concat(Values);
        }

        [Benchmark(Description = "string.join")]
        public string StringJoin()
        {
            return string.Join("", Values);
        }
    }
}