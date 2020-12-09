using BenchmarkDotNet.Attributes;
using System.Numerics;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class VectorVsByteMath
    {
        [Benchmark(Baseline = true, Description = "byte[] test")]
        public void ByteArrayTest()
        {
            byte[] ByteArrayObject = new byte[400000];
            for (int x = 0; x < ByteArrayObject.Length; ++x)
            {
                ByteArrayObject[x] += ByteArrayObject[x];
            }
        }

        [Benchmark(Description = "Struct test")]
        public void StructTest()
        {
            ColorStruct[] StructObject = new ColorStruct[100000];
            for (int x = 0; x < StructObject.Length; ++x)
            {
                StructObject[x].Red += StructObject[x].Red;
                StructObject[x].Green += StructObject[x].Green;
                StructObject[x].Blue += StructObject[x].Blue;
                StructObject[x].Alpha += StructObject[x].Alpha;
            }
        }

        [Benchmark(Description = "Struct UInt test")]
        public void StructUIntTest()
        {
            ColorStruct[] StructObject = new ColorStruct[100000];
            for (int x = 0; x < StructObject.Length; ++x)
            {
                StructObject[x].UIntData += StructObject[x].UIntData;
            }
        }

        [Benchmark(Description = "Vector<byte> test")]
        public void VectorByteTest()
        {
            byte[] ByteArrayObject = new byte[400000];
            var simdLength = Vector<byte>.Count;
            var result = new byte[ByteArrayObject.Length];
            var i = 0;
            for (i = 0; i < ByteArrayObject.Length - simdLength; i += simdLength)
            {
                var va = new Vector<byte>(ByteArrayObject, i);
                (va + va).CopyTo(result, i);
            }
            for (; i < ByteArrayObject.Length; ++i)
            {
                result[i] = (byte)(ByteArrayObject[i] + ByteArrayObject[i]);
            }
        }

        [Benchmark(Description = "Vector<float> test")]
        public void VectorFloatTest()
        {
            float[] FloatArrayObject = new float[100000];
            var simdLength = Vector<float>.Count;
            var result = new float[FloatArrayObject.Length];
            var i = 0;
            for (i = 0; i < FloatArrayObject.Length - simdLength; i += simdLength)
            {
                var va = new Vector<float>(FloatArrayObject, i);
                (va + va).CopyTo(result, i);
            }
            for (; i < FloatArrayObject.Length; ++i)
            {
                result[i] = FloatArrayObject[i] + FloatArrayObject[i];
            }
        }

        [Benchmark(Description = "Vector<uint> test")]
        public void VectorUIntTest()
        {
            uint[] ByteArrayObject = new uint[100000];
            var simdLength = Vector<uint>.Count;
            var result = new uint[ByteArrayObject.Length];
            var i = 0;
            for (i = 0; i < ByteArrayObject.Length - simdLength; i += simdLength)
            {
                var va = new Vector<uint>(ByteArrayObject, i);
                (va + va).CopyTo(result, i);
            }
            for (; i < ByteArrayObject.Length; ++i)
            {
                result[i] = ByteArrayObject[i] + ByteArrayObject[i];
            }
        }
    }
}