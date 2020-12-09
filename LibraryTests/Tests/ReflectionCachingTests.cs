using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class ReflectionCachingTests
    {
        private static ConcurrentDictionary<Type, PropertyInfo[]> ConcurrentDictionary;
        private static Dictionary<Type, PropertyInfo[]> PropertyDictionary;

        [Benchmark(Description = "Get properties with cache")]
        public void CachedReflection()
        {
            var Properties = PropertyCacheFor<TestClass>.Properties;
        }

        [Benchmark(Description = "Get properties with ConcurrentDictionary")]
        public void ConcurrentDictionaryCachedReflection()
        {
            var Properties = ConcurrentDictionary[typeof(TestClass)];
        }

        [Benchmark(Description = "Get properties with dictionary")]
        public void DictionaryCachedReflection()
        {
            var Properties = PropertyDictionary[typeof(TestClass)];
        }

        [Benchmark(Baseline = true, Description = "Get properties without cache")]
        public void NormalReflection()
        {
            var Properties = typeof(TestClass).GetProperties();
        }

        [GlobalSetup]
        public void Setup()
        {
            PropertyDictionary = new Dictionary<Type, PropertyInfo[]>();
            PropertyDictionary.Add(typeof(TestClass), typeof(TestClass).GetProperties());
            ConcurrentDictionary = new ConcurrentDictionary<Type, PropertyInfo[]>();
            ConcurrentDictionary.AddOrUpdate(typeof(TestClass), typeof(TestClass).GetProperties(), (type, properties) => typeof(TestClass).GetProperties());
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