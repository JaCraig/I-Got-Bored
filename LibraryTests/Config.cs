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
            AddDiagnoser(MemoryDiagnoser.Default);
            AddValidator(BenchmarkDotNet.Validators.JitOptimizationsValidator.DontFailOnError);
            AddLogger(BenchmarkDotNet.Loggers.ConsoleLogger.Default);
            AddExporter(BenchmarkDotNet.Exporters.DefaultExporters.Plain);
        }
    }
}