using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public class DictionaryAssignment
    {
        [Benchmark]
        public void AddOrAssign()
        {
            Dictionary<int, string> TestObject = new Dictionary<int, string>();
            AddOrUpdate(TestObject, 1, "Value");
            AddOrUpdate(TestObject, 1, "Value2");
        }

        [Benchmark(Baseline = true)]
        public void Key()
        {
            Dictionary<int, string> TestObject = new Dictionary<int, string>();
            TestObject[1] = "Value";
            TestObject[1] = "Value2";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddOrUpdate(Dictionary<int, string> dic, int key, string newValue)
        {
            if (dic.TryGetValue(key, out var val))
            {
                dic[key] = newValue;
            }
            else
            {
                dic.Add(key, newValue);
            }
        }
    }
}