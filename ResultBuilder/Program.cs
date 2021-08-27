using FileCurator;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ResultBuilder
{
    internal class Program
    {
        private static Regex HeaderData { get; } = new Regex(@"```(?<Header>[\s\S]*)```(?<Table>[\s\S]*)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private static void Main(string[] args)
        {
            new ServiceCollection().AddCanisterModules();
            StringBuilder Builder = new StringBuilder("This is the repository for testing various speed improvements for Structure.Sketching and other libs. The results can be found below the conclussions section.\n\n# Conclussions\n\n# Results\n\n");
            StringBuilder TableBuilder = new StringBuilder();
            var FinalFile = new FileInfo("../../../../README.md");
            var Header = "";
            foreach (var File in new DirectoryInfo("../../../../LibraryTests/bin/Release/net5.0/BenchmarkDotNet.Artifacts/results").EnumerateFiles("*-github.md").OrderBy(x => x.Name))
            {
                var Data = File.Read();
                var Table = HeaderData.Match(Data).Groups["Table"].Value;
                Header = HeaderData.Match(Data).Groups["Header"].Value;
                TableBuilder.AppendLine($"**{File.Name}**").AppendLine().AppendLine(Table).AppendLine("----------");
            }
            Builder.AppendLine($"```{Header}```").AppendLine(TableBuilder.ToString());
            FinalFile.Write(Builder.ToString());
        }
    }
}