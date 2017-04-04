namespace Dotnet.Script.NuGetMetadataResolver.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;    
    using Microsoft.CodeAnalysis.NuGet.Tests;
    using Microsoft.Extensions.Logging;
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

            var packageInstaller = new NuGetPackageInstaller(new CommandRunner(CreateLoggerFactory()),CreatePackageSearcher(), NugetFrameworkProvider.GetFrameworkNameFromAssembly(),CreateLoggerFactory(),directory);

            Dictionary<PackageIdentity, IEnumerable<string>> referencedPackages = new Dictionary<PackageIdentity, IEnumerable<string>>();
            packageInstaller.Install(referencedPackages,new PackageIdentity("System.ComponentModel.TypeConverter", NuGetVersion.Parse("4.0.0")));
        }

        [Fact]
        public void ShouldFallBackToDotNetFolder()
        {
            string directory =
               Path.GetDirectoryName(new Uri(typeof(NuGetMetadataReferenceResolver).GetTypeInfo().Assembly.CodeBase).LocalPath);
            
            var packageInstaller = new NuGetPackageInstaller(new CommandRunner(CreateLoggerFactory()), CreatePackageSearcher(), NugetFrameworkProvider.GetFrameworkNameFromAssembly(),CreateLoggerFactory(), directory);

            Dictionary<PackageIdentity, IEnumerable<string>> referencedPackages = new Dictionary<PackageIdentity, IEnumerable<string>>();
            packageInstaller.Install(referencedPackages, new PackageIdentity("System.ComponentModel.TypeConverter", NuGetVersion.Parse("4.0.0")));

            referencedPackages.First().Value.ShouldNotBeEmpty();

        }


        private static INugetPackageSearcher CreatePackageSearcher()
        {
            string directory =
                Path.GetDirectoryName(new Uri(typeof(NuGetMetadataReferenceResolver).GetTypeInfo().Assembly.CodeBase).LocalPath);
            INugetPackageSearcher packageSearcher = new NuGetPackageSearcher(new NuGetPackageSourceProvider(directory, CreateLoggerFactory()), CreateLoggerFactory());
            return packageSearcher;
        }

        private static ILoggerFactory CreateLoggerFactory()
        {
            return new TestOutPutLoggerFactory();
        }
    }
}
