using Microsoft.CodeAnalysis.Text;

namespace Dotnet.Script.NuGetMetadataResolver.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.CodeAnalysis.NuGet.Tests;
    using Microsoft.Extensions.Logging;
    using Shouldly;
    using Xunit;
    using Xunit.Abstractions;

    public class ScriptProjectProviderTests
    {
        public ScriptProjectProviderTests(ITestOutputHelper testOutputHelper)
        {
            testOutputHelper.Capture();
        }

        [Fact]
        public void ShouldCreateNet46Project()
        {
            ScriptProjectProvider p = ScriptProjectProvider.Create(CreateLoggerFactory());
            var pathToScript = Path.Combine(Path.GetDirectoryName(new Uri(typeof(ScriptParserTests).GetTypeInfo().Assembly.CodeBase)
                .LocalPath),"sample");
            var result = p.CreateProject(pathToScript);
            var json = File.ReadAllText(result.PathToProjectJson);
            json.ShouldContain("net46");
        }

        [Fact]
        public void ShouldCreateNet46Project_FromSource()
        {
            ScriptProjectProvider p = ScriptProjectProvider.Create(CreateLoggerFactory());
            var script = SourceText.From("Console.WriteLine(\"TEST\");");

            var result = p.CreateProject(new []{script});
            var json = File.ReadAllText(result.PathToProjectJson);
            json.ShouldContain("net46");
        }


        private static ILoggerFactory CreateLoggerFactory()
        {
            return new TestOutPutLoggerFactory();
        }
    }

}