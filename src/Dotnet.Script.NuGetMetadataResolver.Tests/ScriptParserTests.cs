namespace Dotnet.Script.NuGetMetadataResolver.Tests
{
    using System.Linq;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.CodeAnalysis.NuGet.Tests;
    using Microsoft.Extensions.Logging;

    using NuGetMetadataResolver;
    using Shouldly;
    using Xunit;
    using Xunit.Abstractions;

    public class ScriptParserTests
    {
        public ScriptParserTests(ITestOutputHelper testOutputHelper)
        {
            testOutputHelper.Capture();
        }

        [Fact]
        public void ShouldResolveSinglePackage()
        {
            var parser = new ScriptParser(CreateLoggerFactory());
            var result = parser.ParseFrom(new[] { "ScriptWithOnePackage.csx" });
            result.PackageReferences.Count.ShouldBe(1);
            result.PackageReferences.Single().Id.ShouldBe("Package");
            result.PackageReferences.Single().Version.ShouldBe("1.0.0");
            result.TargetFramework.ShouldBeNull();
        }

        [Fact]
        public void ShouldResolveMultiplePackages()
        {
            var parser = new ScriptParser(CreateLoggerFactory());
            var result = parser.ParseFrom(new[] { "ScriptWithMultiplePackages.csx" });
            result.PackageReferences.Count.ShouldBe(2);
            result.PackageReferences.First().Id.ShouldBe("Package");
            result.PackageReferences.First().Version.ShouldBe("1.0.0");
            result.PackageReferences.Last().Id.ShouldBe("AnotherPackage");
            result.PackageReferences.Last().Version.ShouldBe("2.0.0");
        }

        [Fact]
        public void ShouldResolveMultiplePackages_FromSingleSource()
        {
            var parser = new ScriptParser(CreateLoggerFactory());
            var script = SourceText.From("#r \"nuget:Package,1.0.0\"\r\n#r \"nuget:AnotherPackage,2.0.0\"\r\n");

            var result = parser.ParseFrom(new[] { script });
            result.PackageReferences.Count.ShouldBe(2);
            result.PackageReferences.First().Id.ShouldBe("Package");
            result.PackageReferences.First().Version.ShouldBe("1.0.0");
            result.PackageReferences.Last().Id.ShouldBe("AnotherPackage");
            result.PackageReferences.Last().Version.ShouldBe("2.0.0");
        }

        [Fact]
        public void ShouldResolveMultiplePackages_FromMultipleSources()
        {
            var parser = new ScriptParser(CreateLoggerFactory());
            var script1 = SourceText.From("#r \"nuget:Package,1.0.0\"");
            var script2 = SourceText.From("#r \"nuget:AnotherPackage,2.0.0\"");

            var result = parser.ParseFrom(new[] { script1, script2 });
            result.PackageReferences.Count.ShouldBe(2);
            result.PackageReferences.First().Id.ShouldBe("Package");
            result.PackageReferences.First().Version.ShouldBe("1.0.0");
            result.PackageReferences.Last().Id.ShouldBe("AnotherPackage");
            result.PackageReferences.Last().Version.ShouldBe("2.0.0");
        }

        [Fact]
        public void ShouldParseTargetFramework()
        {
            var parser = new ScriptParser(CreateLoggerFactory());
            var result = parser.ParseFrom(new[] { "ScriptWithFrameworkReference.csx" });
            result.TargetFramework.ShouldBe("netcoreapp1.0");
        }

        //[Fact]
        //public void ShouldExtractNuGet()
        //{
        //    ScriptProjectProvider p = ScriptProjectProvider.Create(CreateLoggerFactory());
        //    p.CreateProject(Path.GetDirectoryName(new Uri(typeof(ScriptParserTests).GetTypeInfo().Assembly.CodeBase).LocalPath));
        //}


        private static ILoggerFactory CreateLoggerFactory()
        {
            return new TestOutPutLoggerFactory();
        }
    }
}
