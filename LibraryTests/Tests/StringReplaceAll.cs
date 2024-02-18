using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class StringReplaceAll
    {
        [Params(1, 10, 100, 1000, 10000)]
        public int Count { get; set; }

        public string Value { get; set; }

        public char[] Replacements = new char[] { 'A', 'B', 'D', 'E' };

        public Dictionary<string, string> Replacements2 = new()
        {
            ["ABC"] = "C",
            ["DEF"] = "C",
            ["GHI"] = "C",
            ["JKL"] = "C",
        };

        //[Benchmark]
        //public void MultipleReplacements()
        //{
        //    for (var x = 0; x < Count; ++x)
        //    {
        //        Replace(Value, Replacements, 'C');
        //    }
        //}

        [Benchmark]
        public void MultipleReplacementsString()
        {
            for (var x = 0; x < Count; ++x)
            {
                _ = ReplaceAll(Value, Replacements2);
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            for (var x = 0; x < char.MaxValue; ++x)
            {
                Value += (char)x + " ";
            }
        }

        //[Benchmark(Baseline = true)]
        //public void StringReplaceMethod()
        //{
        //    for (var x = 0; x < Count; ++x)
        //    {
        //        _ = Value.Replace('A', 'C').Replace('B', 'C').Replace('D', 'C').Replace('E', 'C');
        //    }
        //}

        [Benchmark(Baseline = true)]
        public void StringReplaceStringMethod()
        {
            for (var x = 0; x < Count; ++x)
            {
                _ = Value.Replace("ABC", "C").Replace("DEF", "C").Replace("GHI", "C").Replace("JKL", "C");
            }
        }

        private string Replace(string value, char[] oldChars, char newChar)
        {
            ReadOnlySpan<char> ValueSpan = value.AsSpan();
            Span<char> Result = new char[value.Length];
            value.CopyTo(Result);
            Span<char> OldCharsSpan = oldChars.AsSpan();
            var CurrentIndex = 0;
            while (true)
            {
                var NextIndex = Result.IndexOfAny(OldCharsSpan);
                if (NextIndex == -1)
                    break;
                if (CurrentIndex != NextIndex)
                    ValueSpan[CurrentIndex..NextIndex].CopyTo(Result[CurrentIndex..NextIndex]);
                Result[NextIndex] = newChar;
            }
            return new string(Result);
        }

        private string ReplaceAll(string? input, Dictionary<string, string>? replacements)
        {
            if (string.IsNullOrEmpty(input) || (replacements?.Count ?? 0) == 0)
                return input ?? "";

            var result = new StringBuilder(input.Length);
            var position = 0;

            foreach (KeyValuePair<string, string> replacement in replacements)
            {
                var key = replacement.Key;
                var value = replacement.Value;
                var index = int.MaxValue;
                while (index > -1)
                {
                    index = input.IndexOf(key, position);
                    if (index < 0)
                    {
                        result.Append(input[position..]);
                        break;
                    }

                    result.Append(input[position..index]);
                    result.Append(value);
                    position = index + key.Length;
                }
                input = result.ToString();
                result.Length = 0;
                position = 0;
            }
            if (position < input.Length)
                result.Append(input[position..]);

            return result.ToString();
        }
    }
}