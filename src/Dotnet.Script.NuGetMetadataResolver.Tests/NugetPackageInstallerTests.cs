namespace Dotnet.Script.NuGetMetadataResolver.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using csx;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.NuGet.Tests;
    using Microsoft.Extensions.Logging;

    using NuGetMetadataResolver;
    using Shouldly;
    using Xunit;
    using Xunit.Abstractions;

    public class NugetPackageInstallerTests
    {
        public NugetPackageInstallerTests(ITestOutputHelper testOutputHelper)
        {
            testOutputHelper.Capture();
        }

        [Fact]
        public void ShouldResolvePackage()
        {
            //var instance = Activator.CreateInstance(typeof(UnresolvedMetadataReference), null,null);
            var constructor = typeof(UnresolvedMetadataReference).GetTypeInfo().DeclaredConstructors.Single();
            var instance = (UnresolvedMetadataReference)constructor.Invoke(new object[] {null, null});
            
            

            var provider = ScriptProjectProvider.Create(CreateLoggerFactory());
            var project = provider.CreateProject(Path.GetDirectoryName(Path.GetFullPath("foo.csx")));
        }

       


        

        private static ILoggerFactory CreateLoggerFactory()
        {
            return new TestOutPutLoggerFactory();
        }
    }
}
