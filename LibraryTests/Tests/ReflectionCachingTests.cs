using BenchmarkDotNet.Attributes;
using System.Reflection;

namespace LibraryTests.Tests
{
    public class ReflectionCachingTests
    {
        [Benchmark(Description = "Get properties with cache")]
        public void CachedReflection()
        {
            var Properties = PropertyCacheFor<TestClass>.Properties;
        }

        [Benchmark(Baseline = true, Description = "Get properties without cache")]
        public void NormalReflection()
        {
            var Properties = typeof(TestClass).GetProperties();
        }

        private static class PropertyCacheFor<T> where T : class
        {
            public static readonly PropertyInfo[] Properties = typeof(T).GetProperties();
        }

        private class TestClass
        {
            public int Property1 { get; set; }

            public int Property2 { get; set; }
        }
    }
}