using BenchmarkDotNet.Attributes;
using System;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public class EnumVsBytePacking
    {
        [Benchmark(Description = "Byte")]
        public void ByteTest()
        {
            var TempValues = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            var Result = (TempValues[0] & 0b00000001) != 0;
            Result = (TempValues[0] & 0b00000010) != 0;
            Result = (TempValues[1] & 0b00000100) != 0;
            Result = (TempValues[2] & 0b00001000) != 0;
            Result = (TempValues[3] & 0b00010000) != 0;
            Result = (TempValues[4] & 0b00100000) != 0;
            Result = (TempValues[5] & 0b01000000) != 0;
            Result = (TempValues[6] & 0b10000000) != 0;
        }

        [Benchmark(Baseline = true, Description = "Enum")]
        public void EnumTest()
        {
            var TempValues = new[] { EnumItem.Value1, EnumItem.Value2, EnumItem.Value3, EnumItem.Value4, EnumItem.Value5, EnumItem.Value6, EnumItem.Value7, EnumItem.Value8 };
            var Result = TempValues[0].HasFlag(EnumItem.Value1);
            Result = TempValues[1].HasFlag(EnumItem.Value2);
            Result = TempValues[2].HasFlag(EnumItem.Value3);
            Result = TempValues[3].HasFlag(EnumItem.Value4);
            Result = TempValues[4].HasFlag(EnumItem.Value5);
            Result = TempValues[5].HasFlag(EnumItem.Value6);
            Result = TempValues[6].HasFlag(EnumItem.Value7);
            Result = TempValues[7].HasFlag(EnumItem.Value8);
        }

        [Benchmark(Description = "UInt")]
        public void UIntTest()
        {
            var TempValues = new uint[] { 1, 2, 4, 8, 16, 32, 64, 128 };
            var Result = (TempValues[0] & 0b00000001) != 0;
            Result = (TempValues[0] & 0b00000010) != 0;
            Result = (TempValues[1] & 0b00000100) != 0;
            Result = (TempValues[2] & 0b00001000) != 0;
            Result = (TempValues[3] & 0b00010000) != 0;
            Result = (TempValues[4] & 0b00100000) != 0;
            Result = (TempValues[5] & 0b01000000) != 0;
            Result = (TempValues[6] & 0b10000000) != 0;
        }

        [Flags]
        private enum EnumItem
        {
            Value1,
            Value2,
            Value3,
            Value4,
            Value5,
            Value6,
            Value7,
            Value8
        }
    }
}