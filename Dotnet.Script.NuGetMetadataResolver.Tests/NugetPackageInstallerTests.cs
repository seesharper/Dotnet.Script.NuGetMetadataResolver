namespace Dotnet.Script.NuGetMetadataResolver.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.CodeAnalysis.NuGet.Tests;
    using NuGet.Packaging.Core;
    using NuGet.Versioning;
    using NuGetMetadataResolver;
    using Xunit;
    using Xunit.Abstractions;

    public class NugetPackageInstallerTests
    {
        public NugetPackageInstallerTests(ITestOutputHelper testOutputHelper)
        {
            testOutputHelper.Capture();
        }


        [Fact]
        public void ShouldInstallPackage()
        {
            string directory = Path.GetDirectoryName(new Uri(typeof(NuGetMetadataReferenceResolver).GetTypeInfo().Assembly.CodeBase).LocalPath);
                                   
            NuGetPackageInstaller installer = new NuGetPackageInstaller(new CommandRunner(TestLoggerFactory.Instance), 
                new NuGetPackageSearcher(
                    new NuGetPackageSourceProvider(TestLoggerFactory.Instance, Directory.GetCurrentDirectory()),
                    TestLoggerFactory.Instance), TestLoggerFactory.Instance, Path.Combine(directory, "packages"));

            installer.Install(new PackageIdentity("LightInject", NuGetVersion.Parse("5.0.0")));
        }
    }
}
