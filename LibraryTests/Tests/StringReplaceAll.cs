using BenchmarkDotNet.Attributes;
using System;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class StringReplaceAll
    {
        [Params(1, 10, 100, 1000, 10000)]
        public int Count { get; set; }

        public string Value { get; set; }

        public char[] Replacements = new char[] { 'A', 'B', 'D', 'E' };

        [Benchmark]
        public void MultipleReplacements()
        {
            for (var x = 0; x < Count; ++x)
            {
                Replace(Value, Replacements, 'C');
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

        [Benchmark(Baseline = true)]
        public void StringReplaceMethod()
        {
            for (var x = 0; x < Count; ++x)
            {
                _ = Value.Replace('A', 'C').Replace('B', 'C').Replace('D', 'C').Replace('E', 'C');
            }
        }

        private string Replace(string value, char[] oldChars, char newChar)
        {
            var ValueSpan = value.AsSpan();
            Span<char> Result = new char[value.Length];
            value.CopyTo(Result);
            var OldCharsSpan = oldChars.AsSpan();
            var CurrentIndex = 0;
            while (true)
            {
                var NextIndex = Result.IndexOfAny(OldCharsSpan);
                if (NextIndex == -1)
                    break;
                if (CurrentIndex != NextIndex)
                    ValueSpan.Slice(CurrentIndex, NextIndex - CurrentIndex).CopyTo(Result.Slice(CurrentIndex, NextIndex - CurrentIndex));
                Result[NextIndex] = newChar;
            }
            return new string(Result);
        }
    }
}