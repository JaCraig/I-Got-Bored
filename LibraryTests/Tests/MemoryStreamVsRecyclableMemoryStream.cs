using BenchmarkDotNet.Attributes;
using Microsoft.IO;
using System.IO;

namespace LibraryTests.Tests
{
    public class MemoryStreamVsRecyclableMemoryStream
    {
        private byte[] Buffer;

        private RecyclableMemoryStreamManager Manager;

        [Params(100, 1000, 10000, 100000, 1000000)]
        public int Size { get; set; }

        [Benchmark(Description = "MemoryStream", Baseline = true)]
        public void MemStreamTests()
        {
            using (MemoryStream TestStream = new MemoryStream())
            {
                TestStream.Write(Buffer, 0, Size);
                var Result = TestStream.ToArray();
            }
        }

        [Benchmark(Description = "RecyclableMemoryStream")]
        public void RecyclableMemoryStreamTests()
        {
            using (var TestStream = Manager.GetStream())
            {
                TestStream.Write(Buffer, 0, Size);
                var Result = TestStream.ToArray();
            }
        }

        [GlobalSetup]
        public void SetUp()
        {
            Buffer = new byte[Size];
            Manager = new RecyclableMemoryStreamManager();
        }
    }
}