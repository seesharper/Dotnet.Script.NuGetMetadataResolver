namespace Dotnet.Script.NuGetMetadataResolver.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Logging;
    using Microsoft.CodeAnalysis.NuGet.Tests;
    using NuGet.Frameworks;
    using NuGet.Packaging.Core;
    using NuGet.Versioning;
    using NuGetMetadataResolver;
    using Shouldly;
    using Xunit;
    using Xunit.Abstractions;

    public class NugetPackageInstallerTests
    {
        public NugetPackageInstallerTests(ITestOutputHelper testOutputHelper)
        {
            LogFactory.Initialize(type => (entry => TestOutputHelper.Current.WriteLine($"{entry.Level} {entry.Message}")));            
            testOutputHelper.Capture();
        }


        [Fact]
        public void ShouldReturnResultForExistingPackage()
        {
            var packageSearcher = CreatePackageSearcher();
            var result = packageSearcher.Search(new PackageIdentity("AutoMapper", NuGetVersion.Parse("6.0.2")));
            result.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldReturnNullForNonExistingPackage()
        {
            var packageSearcher = CreatePackageSearcher();
            var result = packageSearcher.Search(new PackageIdentity(Guid.NewGuid().ToString("N"), NuGetVersion.Parse("5.0.2")));
            result.ShouldBeNull();
        }

        [Fact]
        public void ShouldReturnNullExistingPackageWithInvalidVersion()
        {
            var packageSearcher = CreatePackageSearcher();
            var result = packageSearcher.Search(new PackageIdentity("LightInject", NuGetVersion.Parse("0.0.1")));
            result.ShouldBeNull();
        }

        [Fact]
        public void ShouldInstallPackage()
        {
            string directory =
               Path.GetDirectoryName(new Uri(typeof(NuGetMetadataReferenceResolver).GetTypeInfo().Assembly.CodeBase).LocalPath);

            var packageInstaller = new NuGetPackageInstaller(new CommandRunner(),CreatePackageSearcher(), NugetFrameworkProvider.GetFrameworkNameFromAssembly(),directory);

            packageInstaller.Install(new PackageIdentity("AutoMApper", NuGetVersion.Parse("6.0.2")));
        }
        
        private static INugetPackageSearcher CreatePackageSearcher()
        {
            string directory =
                Path.GetDirectoryName(new Uri(typeof(NuGetMetadataReferenceResolver).GetTypeInfo().Assembly.CodeBase).LocalPath);
            INugetPackageSearcher packageSearcher = new NuGetPackageSearcher(new NuGetPackageSourceProvider(directory));
            return packageSearcher;
        }
    }
}
