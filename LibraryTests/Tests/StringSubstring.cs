using BenchmarkDotNet.Attributes;
using System;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public class StringSubstring
    {
        [Benchmark(Description = "Span.Slice")]
        public void Slice()
        {
            var Result = new string("This is a test of the system".AsSpan().Slice(0, 9).ToArray());
        }

        [Benchmark(Baseline = true, Description = "String.Substring")]
        public void Substring()
        {
            var Result = "This is a test of the system".Substring(0, 9);
        }
    }
}