using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class DynamicTests : TestBaseClass
    {
        [Benchmark(Baseline = true, Description = "Dynamic value")]
        public void Dynamic()
        {
            dynamic Test = new TestClass();
            for (var x = 0; x < Count; ++x)
            {
                string Value = Test.Value;
            }
        }

        [Benchmark(Description = "Dynamic with casting")]
        public void DynamicWithCasting()
        {
            dynamic Test = new TestClass();
            for (var x = 0; x < Count; ++x)
            {
                string Value = ((TestClass)Test).Value;
            }
        }

        [Benchmark(Description = "Static typing")]
        public void StaticTyping()
        {
            var Test = new TestClass();
            for (var x = 0; x < Count; ++x)
            {
                string Value = Test.Value;
            }
        }

        private class TestClass
        {
            public string Value { get; set; }
        }
    }
}