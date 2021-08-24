using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using Microsoft.IO;
using System.IO;

namespace LibraryTests.Tests
{
    public class MemoryStreamVsRecyclableMemoryStream : TestBaseClass
    {
        private byte[] Buffer;

        private RecyclableMemoryStreamManager Manager;

        [Benchmark(Description = "MemoryStream", Baseline = true)]
        public void MemStreamTests()
        {
            using (MemoryStream TestStream = new MemoryStream())
            {
                TestStream.Write(Buffer, 0, Count);
                var Result = TestStream.ToArray();
            }
        }

        [Benchmark(Description = "RecyclableMemoryStream")]
        public void RecyclableMemoryStreamTests()
        {
            using (var TestStream = Manager.GetStream())
            {
                TestStream.Write(Buffer, 0, Count);
                var Result = TestStream.ToArray();
            }
        }

        [GlobalSetup]
        public void SetUp()
        {
            Buffer = new byte[Count];
            Manager = new RecyclableMemoryStreamManager();
        }
    }
}