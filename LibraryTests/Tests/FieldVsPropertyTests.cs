using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class FieldVsPropertyTests : TestBaseClass
    {
        public FieldVsPropertyTests()
        {
            TestObject = new TestClass
            {
                Field = 100,
                Property = 101
            };
        }

        private TestClass TestObject;

        [Benchmark(Description = "Field test")]
        public void FieldTest()
        {
            for (int x = 0; x < Count; ++x)
            {
                var Value = TestObject.Field + TestObject.Field + TestObject.Field;
            }
        }

        [Benchmark(Description = "Local test")]
        public void LocalTest()
        {
            var Data = 100;
            for (int x = 0; x < Count; ++x)
            {
                var Value = Data + Data + Data;
            }
        }

        [Benchmark(Baseline = true, Description = "Property test")]
        public void PropertyTest()
        {
            for (int x = 0; x < Count; ++x)
            {
                var Value = TestObject.Property + TestObject.Property + TestObject.Property;
            }
        }

        private class TestClass
        {
            public int Property { get; set; }
            public int Field;
        }
    }
}