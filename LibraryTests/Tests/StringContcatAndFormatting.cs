using BenchmarkDotNet.Attributes;
using System;
using System.Text;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public class StringFormatting
    {
        private readonly DateTime Now = DateTime.Now;

        [Benchmark(Description = "StringBuilder")]
        public string StringBuilder()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("Today's date is {0:D} and the time is {1:T}", Now, Now);
            return sb.ToString();
        }

        [Benchmark(Description = "string concat")]
        public string StringConcat()
        {
            return "Today's date is " + Now.ToString("D") + " and the time is " + Now.ToString("T");
        }

        [Benchmark(Description = "string.format", Baseline = true)]
        public string StringFormat()
        {
            return string.Format("Today's date is {0:D} and the time is {1:T}", Now, Now);
        }

        [Benchmark(Description = "string interpolation")]
        public string StringInterpolation()
        {
            return $"Today's date is {Now:D} and the time is {Now:T}";
        }

        [Benchmark(Description = "string.replace")]
        public string StringReplace()
        {
            return "Today's date is [Date] and the time is [Time]".Replace("[Date]", Now.ToString("D")).Replace("[Time]", Now.ToString("T"));
        }
    }
}