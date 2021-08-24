using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public class StringTrimTests
    {
        [Benchmark(Description = "String substring trim")]
        public void StringLoop()
        {
            var s = " This is a test ";
            var start = 0;
            var len = s.Length;
            while (start < len && (char.IsWhiteSpace(s[start]) || s[start] == '\u200c'))
                start++;
            var end = len - 1;
            while (end >= start && (char.IsWhiteSpace(s[end]) || s[end] == '\u200c'))
                end--;
            if (start >= len || end < start)
                s = "";
            else
                s = s.Substring(start, end - start + 1);
        }

        [Benchmark(Baseline = true, Description = "String.Trim()")]
        public void StringTrim()
        {
            var s = " This is a test ";
            s.Trim();
        }

        [Benchmark(Description = "String.TrimStart().TrimEnd()")]
        public void StringTrimRightAndLeft()
        {
            var s = " This is a test ";
            s.TrimStart().TrimEnd();
        }
    }
}