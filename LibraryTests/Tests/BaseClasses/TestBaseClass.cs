using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests.BaseClasses
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public abstract class TestBaseClass
    {
        [Params(100, 1000, 10000, 100000)]
        public int Count { get; set; }
    }
}