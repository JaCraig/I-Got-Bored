using BenchmarkDotNet.Attributes;
using System.IO;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public class ReadFile
    {
        private static string InputPath => "../../../../../../../Formats/Text/File.txt";

        [Benchmark(Baseline = true, Description = "File.Read")]
        public void FileRead()
        {
            using (var TestStream = File.Open(InputPath, FileMode.Open))
            {
                byte[] Data = new byte[TestStream.Length];
                TestStream.Read(Data, 0, (int)TestStream.Length);
            }
        }

        [Benchmark(Description = "File.ReadAsync")]
        public void FileReadAsync()
        {
            using (var TestStream = File.Open(InputPath, FileMode.Open))
            {
                byte[] Data = new byte[TestStream.Length];
                TestStream.ReadAsync(Data, 0, (int)TestStream.Length).GetAwaiter().GetResult();
            }
        }

        [Benchmark(Description = "File.Read in loop, 1024")]
        public void FileReadLoop1024()
        {
            byte[] Data = new byte[1024];
            using (var TestStream = File.Open(InputPath, FileMode.Open))
            {
                while (TestStream.Read(Data, 0, 1024) == 1024) { }
            }
        }

        [Benchmark(Description = "File.Read in loop, 1024, async")]
        public void FileReadLoop1024Async()
        {
            byte[] Data = new byte[1024];
            using (var TestStream = File.Open(InputPath, FileMode.Open))
            {
                while (TestStream.ReadAsync(Data, 0, 1024).GetAwaiter().GetResult() == 1024) { }
            }
        }

        [Benchmark(Description = "File.Read in loop, 2048")]
        public void FileReadLoop2048()
        {
            byte[] Data = new byte[2048];
            using (var TestStream = File.Open(InputPath, FileMode.Open))
            {
                while (TestStream.Read(Data, 0, 2048) == 2048) { }
            }
        }

        [Benchmark(Description = "File.Read in loop, 2048, async")]
        public void FileReadLoop2048Async()
        {
            byte[] Data = new byte[2048];
            using (var TestStream = File.Open(InputPath, FileMode.Open))
            {
                while (TestStream.ReadAsync(Data, 0, 2048).GetAwaiter().GetResult() == 2048) { }
            }
        }

        [Benchmark(Description = "File.Read in loop, 4096")]
        public void FileReadLoop4096()
        {
            byte[] Data = new byte[4096];
            using (var TestStream = File.Open(InputPath, FileMode.Open))
            {
                while (TestStream.Read(Data, 0, 4096) == 4096) { }
            }
        }

        [Benchmark(Description = "File.Read in loop, 4096, async")]
        public void FileReadLoop4096Async()
        {
            byte[] Data = new byte[4096];
            using (var TestStream = File.Open(InputPath, FileMode.Open))
            {
                while (TestStream.ReadAsync(Data, 0, 4096).GetAwaiter().GetResult() == 4096) { }
            }
        }

        [Benchmark(Description = "File.OpenRead")]
        public void OpenRead()
        {
            using (var TestStream = File.OpenRead(InputPath))
            {
                byte[] Data = new byte[TestStream.Length];
                TestStream.Read(Data, 0, (int)TestStream.Length);
            }
        }

        [Benchmark(Description = "File.ReadAllBytes")]
        public void ReadAllBytes()
        {
            var Data = File.ReadAllBytes(InputPath);
        }

        [Benchmark(Description = "File.ReadAllBytesAsync")]
        public void ReadAllBytesAsync()
        {
            var Data = File.ReadAllBytesAsync(InputPath);
        }

        [Benchmark(Description = "File.ReadAllBytes static")]
        public void ReadAllBytesStatic()
        {
            byte[] Data = File.ReadAllBytes(InputPath);
        }

        [Benchmark(Description = "File.ReadAllLines")]
        public void ReadAllLines()
        {
            var Data = File.ReadAllLines(InputPath);
        }

        [Benchmark(Description = "File.ReadAllLinesAsync")]
        public void ReadAllLinesAsync()
        {
            var Data = File.ReadAllLinesAsync(InputPath).GetAwaiter().GetResult();
        }

        [Benchmark(Description = "File.ReadAllText")]
        public void ReadAllText()
        {
            var Data = File.ReadAllText(InputPath);
        }

        [Benchmark(Description = "File.ReadAllTextAsync")]
        public void ReadAllTextAsync()
        {
            var Data = File.ReadAllTextAsync(InputPath).GetAwaiter().GetResult();
        }
    }
}