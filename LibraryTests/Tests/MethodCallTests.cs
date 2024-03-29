﻿using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;
using System;
using System.Linq;
using System.Reflection;

namespace LibraryTests.Tests
{
    public class MethodCallTests : TestBaseClass
    {
        private static Func<string> CachedFunc;

        private static Func<string> CachedLambda = () => "A" + "V" + "C";

        [Benchmark(Description = "Func(_=>_)")]
        public void CachedFuncLambda()
        {
            for (int x = 0; x < Count; ++x)
            {
                var Result = CachedLambda();
            }
        }

        [Benchmark(Description = "Func(Method)")]
        public void CachedFuncTest()
        {
            for (int x = 0; x < Count; ++x)
            {
                var Result = CachedFunc();
            }
        }

        [Benchmark(Description = "Direct call to method", Baseline = true)]
        public void DirectCall()
        {
            for (int x = 0; x < Count; ++x)
            {
                var Result = GetResult();
            }
        }

        [Benchmark(Description = "new Func(Method)")]
        public void Func()
        {
            for (int x = 0; x < Count; ++x)
            {
                var Result = new Func<string>(GetResult)();
            }
        }

        [Benchmark(Description = "new Func(_=>_)")]
        public void FuncLambda()
        {
            for (int x = 0; x < Count; ++x)
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
            for (int x = 0; x < Count; ++x)
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
            for (int x = 0; x < Count; ++x)
            {
                var Result = MethodCacheFor<MethodCallTests>.Methods[0].Invoke(this, new object[0]);
            }
        }

        [GlobalSetup]
        public void Setup()
        {
            CachedFunc = new Func<string>(GetResult);
        }

        private static class MethodCacheFor<T> where T : class
        {
            public static readonly MethodInfo[] Methods = typeof(T).GetMethods().Where(x => x.Name == "GetResult").ToArray();
        }
    }
}