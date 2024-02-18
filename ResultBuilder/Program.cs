using BigBook;
using FileCurator;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ResultBuilder
{
    internal static class Program
    {
        private static Regex HeaderData { get; } = new Regex(@"```(?<Header>[\s\S]*)```(?<Table>[\s\S]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private static void Main(string[] args)
        {
            _ = new ServiceCollection().AddCanisterModules();
            var Builder = new StringBuilder("This is a repository for testing various speed improvements for my various libraries and to test out general improvements between versions of .Net. The results can be found below the conclussions section.\n\n# Conclussions\n\n# Results\n\n");
            var TableBuilder = new StringBuilder();
            var FinalFile = new FileInfo("../../../../README.md");
            var Header = "";
            foreach (FileCurator.Interfaces.IFile File in new DirectoryInfo("../../../../LibraryTests/bin/Release/net8.0/BenchmarkDotNet.Artifacts/results").EnumerateFiles("*-github.md").OrderBy(x => x.Name))
            {
                var Data = File.Read();
                var Table = HeaderData.Match(Data).Groups["Table"].Value;
                Header = HeaderData.Match(Data).Groups["Header"].Value;
                _ = TableBuilder.AppendLine($"**{SplitCamelCase(File.Name)}**").AppendLine().AppendLine(Table).AppendLine("----------");
            }
            _ = Builder.AppendLine($"```{Header}```").AppendLine(TableBuilder.ToString());
            _ = FinalFile.Write(Builder.ToString());
        }

        /// <summary>
        /// Splits the camel case.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Splits the camel case names</returns>
        private static string SplitCamelCase(string input)
        {
            input = input.Replace("LibraryTests.Tests.", "").Replace("-report-github.md", "");
            return input?.AddSpaces() ?? "";
        }
    }
}