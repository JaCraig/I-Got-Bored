﻿using BenchmarkDotNet.Attributes;
using BigBook;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Dynamic;

namespace LibraryTests.Tests
{
    [HtmlExporter, RankColumn, MemoryDiagnoser]
    public class DynamoTests
    {
        public object[] Data { get; set; }

        private object AnonymousType { get; set; }

        private ExpandoObject ExpandoType { get; set; }

        private TestClass TestClassValue { get; set; }

        [Params(1, 10, 100, 1000, 10000)]
        public int Count;

        [Benchmark]
        public object[] AnnonymousTest()
        {
            Data = new object[Count];
            for (var x = 0; x < Count; ++x)
            {
                Data[x] = new { A = "Test data goes here" };
            }
            return Data;
        }

        [Benchmark(Baseline = true)]
        public object[] ClassTest()
        {
            Data = new TestClass[Count];
            for (var x = 0; x < Count; ++x)
            {
                Data[x] = new TestClass { A = "Test data goes here" };
            }
            return Data;
        }

        [Benchmark]
        public object[] ConcurrentDictionaryConversionTest()
        {
            Data = new ConcurrentDictionary<string, object>[Count];
            for (var x = 0; x < Count; ++x)
            {
                var Temp = new ConcurrentDictionary<string, object>();
                Temp.AddOrUpdate("A", "Test data goes here", (_, __) => "Test data goes here");
                Data[x] = Temp;
            }
            return Data;
        }

        [Benchmark]
        public object[] DynamoAnonymousConversionTest()
        {
            Data = new Dynamo[Count];
            for (var x = 0; x < Count; ++x)
            {
                Data[x] = new Dynamo(AnonymousType);
            }
            return Data;
        }

        [Benchmark]
        public object[] DynamoConversionTest()
        {
            Data = new Dynamo[Count];
            for (var x = 0; x < Count; ++x)
            {
                Data[x] = new Dynamo(TestClassValue);
            }
            return Data;
        }

        [Benchmark]
        public object[] DynamoExpandoConversionTest()
        {
            Data = new Dynamo[Count];
            for (var x = 0; x < Count; ++x)
            {
                Data[x] = new Dynamo(ExpandoType);
            }
            return Data;
        }

        [Benchmark]
        public object[] DynamoTest()
        {
            Data = new Dynamo[Count];
            for (var x = 0; x < Count; ++x)
            {
                dynamic Temp = new Dynamo();
                Temp.A = "Test data goes here";
                Data[x] = Temp;
            }
            return Data;
        }

        [Benchmark]
        public object[] ExpandoTest()
        {
            Data = new ExpandoObject[Count];
            for (var x = 0; x < Count; ++x)
            {
                dynamic Temp = new ExpandoObject();
                Temp.A = "Test data goes here";
                Data[x] = Temp;
            }
            return Data;
        }

        [GlobalSetup]
        public void Setup()
        {
            new ServiceCollection().AddCanisterModules();
            AnonymousType = new { A = "Test data goes here" };
            dynamic Temp = new ExpandoObject();
            Temp.A = "Test data goes here";
            ExpandoType = Temp;
            TestClassValue = new TestClass { A = "Test data goes here" };
        }

        private class TestClass
        {
            public string A { get; set; }
        }
    }
}