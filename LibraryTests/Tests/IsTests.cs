﻿using BenchmarkDotNet.Attributes;
using BigBook;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryTests.Tests
{
    [MemoryDiagnoser, HtmlExporter, MarkdownExporter]
    public class IsTests
    {
        private TestClass[] Data { get; set; }

        [Benchmark]
        public void IsAssignableTest()
        {
            _ = Data.GetType().IsAssignableFrom(typeof(ITestClass[]));
        }

        [Benchmark]
        public void IsMethodTest()
        {
            _ = Data.Is<ITestClass[]>();
        }

        [Benchmark(Baseline = true)]
        public void IsTest()
        {
            _ = Data is ITestClass[];
        }

        [Benchmark]
        public void IsTypeTest()
        {
            _ = Data.GetType().Is<ITestClass[]>();
        }

        [GlobalSetup]
        public void Setup()
        {
            Data = new TestClass[100];
            new ServiceCollection().AddCanisterModules();
        }

        private interface ITestClass
        {
        }

        private class TestClass : ITestClass
        {
        }
    }
}