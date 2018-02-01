using BenchmarkDotNet.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace LibraryTests.Tests
{
    public class MethodCallTests
    {
        [Benchmark(Description = "Direct call to method", Baseline = true)]
        public void DirectCall()
        {
            for (int x = 0; x < 1000; ++x)
            {
                var Result = GetResult();
            }
        }

        [Benchmark(Description = "Func(Method)")]
        public void Func()
        {
            for (int x = 0; x < 1000; ++x)
            {
                var Result = new Func<string>(GetResult)();
            }
        }

        [Benchmark(Description = "Func(_=>_)")]
        public void FuncLambda()
        {
            for (int x = 0; x < 1000; ++x)
            {
                var Result = new Func<string>(() => "A" + "V" + "C")();
            }
        }

        public string GetResult()
        {
            return "A" + "V" + "C";
        }

        [Benchmark(Description = "Local method")]
        public void LocalFunction()
        {
            for (int x = 0; x < 1000; ++x)
            {
                var Result = GetResultsLocal();
            }

            string GetResultsLocal()
            {
                return "A" + "V" + "C";
            }
        }

        [Benchmark(Description = "MethodInfo, cached")]
        public void MethodInfoInvoke()
        {
            for (int x = 0; x < 1000; ++x)
            {
                var Result = MethodCacheFor<MethodCallTests>.Methods[0].Invoke(this, new object[0]);
            }
        }

        private static class MethodCacheFor<T> where T : class
        {
            public static readonly MethodInfo[] Methods = typeof(T).GetMethods().Where(x => x.Name == "GetResult").ToArray();
        }
    }
}