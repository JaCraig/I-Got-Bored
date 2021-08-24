using BenchmarkDotNet.Attributes;
using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, AllStatisticsColumn, RankColumn, RPlotExporter]
    public class ObjectCreationTests
    {
        private Func<TestClass> CompiledLambda;

        private ConstructorInfo TestClassConstructor;

        [Benchmark(Description = "Create object using Activator.CreateInstance<T>()")]
        public void ActivatorGenericTest()
        {
            var Item = Activator.CreateInstance<TestClass>();
        }

        [Benchmark(Description = "Create object using Activator.CreateInstance()")]
        public void ActivatorTest()
        {
            var Item = Activator.CreateInstance(typeof(TestClass));
        }

        [Benchmark(Description = "Create object using ConstructorInfo, cached in field")]
        public void CachedConstructorInfoTest()
        {
            var Item = TestClassConstructor.Invoke(new object[0]);
        }

        [Benchmark(Description = "Create object using compiled lambda expression, cached")]
        public void CachedLambdaExpressionTest()
        {
            var Item = CompiledLambda();
        }

        [Benchmark(Description = "Create object using ConstructorInfo cached")]
        public void ConstructorInfoCacheTest()
        {
            var Item = ConstructorInfoCacheFor<TestClass>.Constructors[0].Invoke(new object[0]);
        }

        [Benchmark(Description = "Create object using ConstructorInfo")]
        public void ConstructorInfoTest()
        {
            var Item = typeof(TestClass).GetConstructors()[0].Invoke(new object[0]);
        }

        [Benchmark(Description = "Create object using FormatterServices")]
        public void FormatterServicesTest()
        {
            var Item = FormatterServices.GetUninitializedObject(typeof(TestClass));
        }

        [Benchmark(Description = "Create object using compiled lambda expression")]
        public void LambdaExpressionTest()
        {
            var Item = Expression.Lambda<Func<object>>(Expression.New(typeof(TestClass))).Compile()();
        }

        [Benchmark(Description = "Create object using new", Baseline = true)]
        public void NewTest()
        {
            var Item = new TestClass();
        }

        [GlobalSetup]
        public void Setup()
        {
            CompiledLambda = Expression.Lambda<Func<TestClass>>(Expression.New(typeof(TestClass))).Compile();
            TestClassConstructor = typeof(TestClass).GetConstructors()[0];
        }

        private static class ConstructorInfoCacheFor<T> where T : class
        {
            public static readonly ConstructorInfo[] Constructors = typeof(T).GetConstructors();
        }

        private class TestClass
        {
            public int Property { get; set; }
        }
    }
}