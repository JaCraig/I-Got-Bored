using BenchmarkDotNet.Attributes;

namespace LibraryTests.Tests
{
    public class DynamicTests
    {
        [Benchmark(Baseline = true, Description = "Dynamic value")]
        public void Dynamic()
        {
            dynamic Test = new TestClass();
            for (var x = 0; x < 1000; ++x)
            {
                string Value = Test.Value;
            }
        }

        [Benchmark(Description = "Dynamic with casting")]
        public void DynamicWithCasting()
        {
            dynamic Test = new TestClass();
            for (var x = 0; x < 1000; ++x)
            {
                string Value = ((TestClass)Test).Value;
            }
        }

        [Benchmark(Description = "Static typing")]
        public void StaticTyping()
        {
            var Test = new TestClass();
            for (var x = 0; x < 1000; ++x)
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