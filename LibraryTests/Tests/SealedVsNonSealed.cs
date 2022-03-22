using BenchmarkDotNet.Attributes;
using LibraryTests.Tests.BaseClasses;

namespace LibraryTests.Tests
{
    public class SealedVsNonSealed : TestBaseClass
    {
        private readonly AbstractSealedType AbstractSealedType = new();
        private readonly AbstractType AbstractType = new();
        private readonly NonSealedType NonSealedType = new();
        private readonly NotVirtualSealed NotVirtualSealedType = new();
        private readonly NotVirtual NotVirtualType = new();
        private readonly SealedType SealedType = new();

        [Benchmark]
        public void Abstract()
        {
            AbstractType.Method();
        }

        [Benchmark]
        public void AbstractSealed()
        {
            AbstractSealedType.Method();
        }

        [Benchmark]
        public void NonSealed()
        {
            NonSealedType.Method();
        }

        [Benchmark(Baseline = true)]
        public void NotVirtual()
        {
            for (var x = 0; x < Count; ++x)
            {
                NotVirtualType.Method();
            }
        }

        [Benchmark(Description = "Normal class ")]
        public void NotVirtualSealed()
        {
            for (var x = 0; x < Count; ++x)
            {
                NotVirtualSealedType.Method();
            }
        }

        [Benchmark]
        public void Sealed()
        {
            SealedType.Method();
        }
    }

    internal abstract class AbstractBaseType
    {
        public abstract void Method();
    }

    internal sealed class AbstractSealedType : AbstractBaseType
    {
        public override void Method()
        {
        }
    }

    internal class AbstractType : AbstractBaseType
    {
        public override void Method()
        {
        }
    }

    internal class BaseType
    {
        public virtual void Method()
        { }
    }

    internal class NonSealedType : BaseType
    {
        public override void Method()
        { }
    }

    internal class NotVirtual
    {
        public void Method()
        { }
    }

    internal sealed class NotVirtualSealed
    {
        public void Method()
        { }
    }

    internal sealed class SealedType : BaseType
    {
        public override void Method()
        { }
    }
}