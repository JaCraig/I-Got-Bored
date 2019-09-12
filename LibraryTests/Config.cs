using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;

namespace LibraryTests
{
    /// <summary>
    /// Benchmark config
    /// </summary>
    /// <seealso cref="BenchmarkDotNet.Configs.ManualConfig"/>
    public class Config : ManualConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        public Config()
        {
            Add(MemoryDiagnoser.Default);
            Add(BenchmarkDotNet.Validators.JitOptimizationsValidator.DontFailOnError);
            Add(BenchmarkDotNet.Loggers.ConsoleLogger.Default);
            Add(BenchmarkDotNet.Exporters.DefaultExporters.Plain);
        }
    }
}